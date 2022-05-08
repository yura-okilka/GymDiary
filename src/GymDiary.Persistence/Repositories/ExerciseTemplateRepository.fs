namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Dtos
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseTemplateRepository =

    let private unwrapId id =
        let rawId = ExerciseTemplateId.value id
        (rawId, $"ExerciseTemplate with id '%s{rawId}'")

    let create (collection: IMongoCollection<ExerciseTemplateDto>) (entity: ExerciseTemplate) =
        task {
            try
                let dto = entity |> ExerciseTemplateDto.fromDomain
                do! collection.InsertOneAsync(dto)

                return
                    dto.Id
                    |> ExerciseTemplateId.create (nameof dto.Id)
                    |> Result.mapError (PersistenceError.dtoConversion "ExerciseTemplateId")
            with
            | ex -> return PersistenceError.fromException "create ExerciseTemplate" ex
        }
        |> Async.AwaitTask

    let getById (collection: IMongoCollection<ExerciseTemplateDto>) (id: ExerciseTemplateId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! dto = collection.Find(fun d -> d.Id = id).SingleOrDefaultAsync()

                if isNull dto then
                    return PersistenceError.notFoundResult entityWithIdMsg
                else
                    return
                        dto
                        |> ExerciseTemplateDto.toDomain
                        |> Result.mapError (PersistenceError.dtoConversion "ExerciseTemplateDto")
            with
            | ObjectIdFormatException _ -> return PersistenceError.notFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"get %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask

    let update (collection: IMongoCollection<ExerciseTemplateDto>) (entity: ExerciseTemplate) =
        task {
            let _, entityWithIdMsg = unwrapId entity.Id

            try
                let dto = entity |> ExerciseTemplateDto.fromDomain
                let! result = collection.ReplaceOneAsync((fun d -> d.Id = dto.Id), dto)

                if result.ModifiedCount = 0 then
                    return PersistenceError.notFoundResult entityWithIdMsg
                else
                    return Ok()
            with
            | ObjectIdFormatException _ -> return PersistenceError.notFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"update %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask

    let delete (collection: IMongoCollection<ExerciseTemplateDto>) (id: ExerciseTemplateId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! result = collection.DeleteOneAsync(fun d -> d.Id = id)

                if result.DeletedCount = 0 then
                    return PersistenceError.notFoundResult entityWithIdMsg
                else
                    return Ok()
            with
            | ObjectIdFormatException _ -> return PersistenceError.notFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"delete %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask
