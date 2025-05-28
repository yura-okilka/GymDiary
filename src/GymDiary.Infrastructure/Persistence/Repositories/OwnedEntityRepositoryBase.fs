namespace GymDiary.Infrastructure.Persistence.Repositories

open FsToolkit.ErrorHandling
open GymDiary.Application.Persistence
open GymDiary.Application.Time
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

[<AbstractClass>]
type OwnedEntityRepositoryBase<'TEntity, 'TDocument when 'TDocument :> IDocumentWithOwner>
    (mapper: IDocumentMapper<'TEntity, 'TDocument>, clock: IClock) =
    inherit EntityRepositoryBase<'TEntity, 'TDocument>(mapper, clock)

    interface IOwnedEntityRepository<'TEntity> with

        member r.GetOneByOwner id ownerId = task {
            let id = id.Value
            let ownerId = ownerId.Value

            let! documentOption =
                r.Collection
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionExceptionV2(typeof<'TDocument>.Name, error)))
        }

        member r.GetAllByOwner ownerId = task {
            let ownerId = ownerId.Value

            let! documents = r.Collection.Find(fun d -> d.OwnerId = ownerId).ToListAsync()

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionExceptionV2(typeof<'TDocument>.Name, error)))
        }
