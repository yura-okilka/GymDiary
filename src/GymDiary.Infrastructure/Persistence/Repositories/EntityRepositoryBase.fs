namespace GymDiary.Infrastructure.Persistence.Repositories

open FsToolkit.ErrorHandling
open GymDiary.Application.Persistence
open GymDiary.Application.Time
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

[<AbstractClass>]
type EntityRepositoryBase<'TEntity, 'TDocument when 'TDocument :> IDocument>(mapper: IDocumentMapper<'TEntity, 'TDocument>, clock: IClock) =

    abstract member Collection: IMongoCollection<'TDocument>

    interface IEntityRepository<'TEntity> with

        member r.Create entity = task {
            let document = mapper.MapFromDomain entity

            let utcNow = clock.UtcNow
            document.CreatedOnUtc <- utcNow
            document.UpdatedOnUtc <- utcNow

            do! r.Collection.InsertOneAsync(document)
        }

        member r.Update id entity = taskResult {
            let id = id.Value
            let document = mapper.MapFromDomain entity

            let utcNow = clock.UtcNow
            document.UpdatedOnUtc <- utcNow

            let! result = r.Collection.ReplaceOneAsync((fun d -> d.Id = id), document)

            if result.ModifiedCount = 0 then
                return! Error(EntityNotFound(typeof<'TEntity>.Name, id))
        }

        member r.Delete id = task {
            let id = id.Value
            let! _ = r.Collection.DeleteOneAsync(fun d -> d.Id = id)
            return ()
        }

        member r.Get id = task {
            let id = id.Value
            let! documentOption = r.Collection.Find(fun d -> d.Id = id).SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionExceptionV2(typeof<'TDocument>.Name, error)))
        }

        member r.ExistsWithId id =
            let id = id.Value
            r.Collection.Find(fun d -> d.Id = id).AnyAsync()
