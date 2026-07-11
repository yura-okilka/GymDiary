namespace GymDiary.Persistence.MongoDB.Repositories

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open Microsoft.Extensions.Logging
open MongoDB.Driver

// Impure database actions are logged here, at the boundary where they actually happen (Seemann's
// "repeatable execution"): writes at Information, reads at Debug. Both the input and the outcome
// are logged so a run can be reconstructed. Pure decisions stay unlogged in the workflows.
[<AbstractClass>]
type EntityRepositoryBase<'TEntity, [<Measure>] 'm, 'TDocument when 'TDocument :> IDocument>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>, logger: ILogger) =
    let entityName = typeof<'TEntity>.Name

    interface IEntityRepository<'TEntity, Guid<'m>> with
        member _.Create entity = async {
            let document = mapper.MapFromDomain entity
            do! collection.InsertOneAsync(document) |> Async.AwaitTask
            logger.LogInformation("Created {Entity} {Id}", entityName, document.Id)
        }

        member _.Update entity = asyncResult {
            let document = mapper.MapFromDomain entity
            let! result = collection.ReplaceOneAsync((fun d -> d.Id = document.Id), document) |> Async.AwaitTask

            if result.ModifiedCount = 0 then
                logger.LogInformation("Update matched no {Entity} {Id}", entityName, document.Id)
                return! Error(EntityNotFound(entityName, string document.Id))

            logger.LogInformation("Updated {Entity} {Id}", entityName, document.Id)
        }

        member _.Delete(id: Guid<'m>) = async {
            let id = %id
            let! result = collection.DeleteOneAsync(fun d -> d.Id = id) |> Async.AwaitTask

            if result.DeletedCount > 0L then
                logger.LogInformation("Deleted {Entity} {Id}", entityName, id)
            else
                logger.LogInformation("Delete matched no {Entity} {Id}", entityName, id)
        }

        member _.Get(id: Guid<'m>) = async {
            let id = %id
            let! documentOption = collection.Find(fun d -> d.Id = id).SingleOrNoneAsync() |> Async.AwaitTask

            logger.LogDebug(
                "Retrieved {Entity} {Id}: {Outcome}",
                entityName,
                id,
                (if Option.isSome documentOption then "found" else "not found")
            )

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.ExistsWithId(id: Guid<'m>) = async {
            let id = %id
            let! exists = collection.Find(fun d -> d.Id = id).AnyAsync() |> Async.AwaitTask
            logger.LogDebug("Checked {Entity} {Id} existence: {Exists}", entityName, id, exists)
            return exists
        }
