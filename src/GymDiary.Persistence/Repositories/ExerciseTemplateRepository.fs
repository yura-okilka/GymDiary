namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Core.Domain.Logic
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseTemplateRepository =

    let private templateWithIdMsg id = $"ExerciseTemplate with id '%s{id}'"

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
        (SportsmanId ownerId)
        (ExerciseTemplateId templateId)
        =
        asyncResult {
            let entityWithIdMsg = templateWithIdMsg templateId

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
            let entityWithIdMsg = templateWithIdMsg (entity.Id |> ExerciseTemplateId.value)
            let dto = entity |> ExerciseTemplateDto.fromDomain

            let! result =
                dto
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = dto.Id))
                |> AsyncResult.mapError (PersistenceError.fromException $"update %s{entityWithIdMsg}")

            if result.ModifiedCount = 0L then
                return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
        }

    let delete (collection: IMongoCollection<ExerciseTemplateDto>) (ExerciseTemplateId templateId) =
        asyncResult {
            let entityWithIdMsg = templateWithIdMsg templateId

            let! result =
                MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = templateId))
                |> AsyncResult.mapError (PersistenceError.fromException $"delete %s{entityWithIdMsg}")

            if result.DeletedCount = 0L then
                return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
        }
