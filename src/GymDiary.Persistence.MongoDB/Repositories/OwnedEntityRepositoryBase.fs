namespace GymDiary.Persistence.MongoDB.Repositories

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open MongoDB.Driver

[<AbstractClass>]
type OwnedEntityRepositoryBase<'TEntity, [<Measure>] 'm, 'TDocument when 'TDocument :> IDocumentWithOwner>
    (collection: IMongoCollection<'TDocument>, mapper: IDocumentMapper<'TEntity, 'TDocument>) =
    inherit EntityRepositoryBase<'TEntity, 'm, 'TDocument>(collection, mapper)
    interface IOwnedEntityRepository<'TEntity, Guid<'m>> with
        member _.GetOneByOwner (id: Guid<'m>) (ownerId: UserId) = task {
            let id = UMX.untag id
            let ownerId = UMX.untag ownerId

            let! documentOption =
                collection
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.GetAllByOwner(ownerId: UserId) = task {
            let ownerId = UMX.untag ownerId
            let! documents = collection.Find(fun d -> d.OwnerId = ownerId).ToListAsync()

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM mapper.MapToDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<'TDocument>.Name, error)))
        }

        member _.DeleteByOwner (id: Guid<'m>) (ownerId: UserId) = task {
            let id = UMX.untag id
            let ownerId = UMX.untag ownerId
            let! result = collection.DeleteOneAsync(fun d -> d.Id = id && d.OwnerId = ownerId)
            return result.DeletedCount > 0L
        }
