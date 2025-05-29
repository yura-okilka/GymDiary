namespace GymDiary.Infrastructure.Persistence.Repositories

open FsToolkit.ErrorHandling
open GymDiary.Application.Persistence
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

[<AbstractClass>]
type EntityRepositoryBase<'TEntity, 'TDocument when 'TDocument :> IDocument>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>) =
    interface IEntityRepository<'TEntity> with
        member _.Create entity = task {
            let document = mapper.MapFromDomain entity
            do! collection.InsertOneAsync(document)
        }

        member _.Update entity = taskResult {
            let document = mapper.MapFromDomain entity
            let! result = collection.ReplaceOneAsync((fun d -> d.Id = document.Id), document)

            if result.ModifiedCount = 0 then
                return! Error(EntityNotFound(typeof<'TEntity>.Name, document.Id))
        }

        member _.Delete id = task {
            let id = id.Value
            let! _ = collection.DeleteOneAsync(fun d -> d.Id = id)
            return ()
        }

        member _.Get id = task {
            let id = id.Value
            let! documentOption = collection.Find(fun d -> d.Id = id).SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.ExistsWithId id =
            let id = id.Value
            collection.Find(fun d -> d.Id = id).AnyAsync()
