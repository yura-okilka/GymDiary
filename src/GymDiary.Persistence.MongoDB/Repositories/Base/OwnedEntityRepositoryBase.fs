namespace GymDiary.Persistence.MongoDB.Repositories

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open Microsoft.Extensions.Logging
open MongoDB.Driver

[<AbstractClass>]
type OwnedEntityRepositoryBase<'TEntity, [<Measure>] 'm, 'TDocument when 'TDocument :> IDocumentWithOwner>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>, logger: ILogger) =
    inherit EntityRepositoryBase<'TEntity, 'm, 'TDocument>(collection, mapper, logger)
    let entityName = typeof<'TEntity>.Name

    interface IOwnedEntityRepository<'TEntity, Guid<'m>> with
        member _.GetOneByOwner (id: Guid<'m>) (ownerId: UserId) = async {
            let id = %id
            let ownerId = %ownerId

            let! documentOption =
                collection
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()
                |> Async.AwaitTask

            logger.LogDebug(
                "Retrieved {Entity} {Id} for owner {OwnerId}: {Outcome}",
                entityName,
                id,
                ownerId,
                (if Option.isSome documentOption then "found" else "not found")
            )

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.GetAllByOwner(ownerId: UserId) = async {
            let ownerId = %ownerId
            let! documents = collection.Find(fun d -> d.OwnerId = ownerId).ToListAsync() |> Async.AwaitTask
            let documents = List.ofSeq documents

            logger.LogDebug("Retrieved {Count} {Entity} for owner {OwnerId}", List.length documents, entityName, ownerId)

            return
                documents
                |> List.traverseResultM mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.DeleteByOwner (id: Guid<'m>) (ownerId: UserId) = async {
            let id = %id
            let ownerId = %ownerId
            let! result = collection.DeleteOneAsync(fun d -> d.Id = id && d.OwnerId = ownerId) |> Async.AwaitTask
            let deleted = result.DeletedCount > 0L

            if deleted then
                logger.LogInformation("Deleted {Entity} {Id} for owner {OwnerId}", entityName, id, ownerId)
            else
                logger.LogInformation("Delete matched no {Entity} {Id} for owner {OwnerId}", entityName, id, ownerId)

            return deleted
        }
