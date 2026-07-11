namespace GymDiary.Persistence.MongoDB.Repositories

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open MongoDB.Driver

[<AbstractClass>]
type EntityRepositoryBase<'TEntity, [<Measure>] 'm, 'TDocument when 'TDocument :> IDocument>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>) =
    interface IEntityRepository<'TEntity, Guid<'m>> with
        member _.Create entity = async {
            let document = mapper.MapFromDomain entity
            do! collection.InsertOneAsync(document) |> Async.AwaitTask
        }

        member _.Update entity = asyncResult {
            let document = mapper.MapFromDomain entity
            let! result = collection.ReplaceOneAsync((fun d -> d.Id = document.Id), document) |> Async.AwaitTask

            if result.ModifiedCount = 0 then
                return! Error(EntityNotFound(typeof<'TEntity>.Name, string document.Id))
        }

        member _.Delete(id: Guid<'m>) = async {
            let id = %id
            let! _ = collection.DeleteOneAsync(fun d -> d.Id = id) |> Async.AwaitTask
            return ()
        }

        member _.Get(id: Guid<'m>) = async {
            let id = %id
            let! documentOption = collection.Find(fun d -> d.Id = id).SingleOrNoneAsync() |> Async.AwaitTask

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.ExistsWithId(id: Guid<'m>) =
            let id = %id
            collection.Find(fun d -> d.Id = id).AnyAsync() |> Async.AwaitTask
