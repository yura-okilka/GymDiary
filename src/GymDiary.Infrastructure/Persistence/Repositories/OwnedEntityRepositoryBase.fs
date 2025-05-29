namespace GymDiary.Infrastructure.Persistence.Repositories

open FsToolkit.ErrorHandling
open GymDiary.Application.Persistence
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

[<AbstractClass>]
type OwnedEntityRepositoryBase<'TEntity, 'TDocument when 'TDocument :> IDocumentWithOwner>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>) =
    inherit EntityRepositoryBase<'TEntity, 'TDocument>(collection, mapper)

    interface IOwnedEntityRepository<'TEntity> with
        member _.GetOneByOwner id ownerId = task {
            let id = id.Value
            let ownerId = ownerId.Value

            let! documentOption =
                collection
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.GetAllByOwner ownerId = task {
            let ownerId = ownerId.Value
            let! documents = collection.Find(fun d -> d.OwnerId = ownerId).ToListAsync()

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }
