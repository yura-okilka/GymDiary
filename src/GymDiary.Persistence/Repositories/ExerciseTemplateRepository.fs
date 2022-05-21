namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseTemplateRepository =

    let private unwrapId id =
        let rawId = ExerciseTemplateId.value id
        (rawId, $"ExerciseTemplate with id '%s{rawId}'")

    let create (collection: IMongoCollection<ExerciseTemplateDto>) (entity: ExerciseTemplate) =
        asyncResult {
            let! createdDto =
                entity
                |> ExerciseTemplateDto.fromDomain
                |> MongoRepository.insertOne collection
                |> AsyncResult.mapError (PersistenceError.fromException "create ExerciseTemplate")

            return!
                createdDto.Id
                |> ExerciseTemplateId.create (nameof createdDto.Id)
                |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseTemplateId>.Name)
                |> Async.singleton
        }

    let getById
        (collection: IMongoCollection<ExerciseTemplateDto>)
        (ownerId: SportsmanId)
        (templateId: ExerciseTemplateId)
        =
        asyncResult {
            let templateId, entityWithIdMsg = unwrapId templateId
            let ownerId = ownerId |> SportsmanId.value

            let! dtoOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = templateId && d.OwnerId = ownerId))
                |> AsyncResult.mapError (PersistenceError.fromException $"get %s{entityWithIdMsg}")

            match dtoOption with
            | None -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | Some dto ->
                return!
                    dto
                    |> ExerciseTemplateDto.toDomain
                    |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseTemplateDto>.Name)
                    |> Async.singleton
        }

    let update (collection: IMongoCollection<ExerciseTemplateDto>) (entity: ExerciseTemplate) =
        asyncResult {
            let _, entityWithIdMsg = unwrapId entity.Id
            let dto = entity |> ExerciseTemplateDto.fromDomain

            let! result =
                dto
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = dto.Id))
                |> AsyncResult.mapError (PersistenceError.fromException $"update %s{entityWithIdMsg}")

            match result with
            | { ModifiedCount = 0L } -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | _ -> return ()
        }

    let delete (collection: IMongoCollection<ExerciseTemplateDto>) (id: ExerciseTemplateId) =
        asyncResult {
            let id, entityWithIdMsg = unwrapId id

            let! result =
                MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = id))
                |> AsyncResult.mapError (PersistenceError.fromException $"delete %s{entityWithIdMsg}")

            match result with
            | { DeletedCount = 0L } -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | _ -> return ()
        }
