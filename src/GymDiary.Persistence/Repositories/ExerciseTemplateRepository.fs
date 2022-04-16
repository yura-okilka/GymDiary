namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Persistence.ExerciseTemplate
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

    let getById (collection: IMongoCollection<ExerciseTemplateDto>) (id: ExerciseTemplateId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! cursor = collection.FindAsync(fun d -> d.Id = id)
                let! dto = cursor.SingleOrDefaultAsync()

                if isNull dto then
                    return PersistenceError.notFound entityWithIdMsg |> Error
                else
                    return
                        dto
                        |> ExerciseTemplateDto.toDomain
                        |> Result.mapError (PersistenceError.dtoConversion "ExerciseTemplateDto")
            with
            | ex -> return PersistenceError.fromException $"get %s{entityWithIdMsg}" ex
        }

    let update (collection: IMongoCollection<ExerciseTemplateDto>) (entity: ExerciseTemplate) =
        task {
            let _, entityWithIdMsg = unwrapId entity.Id

            try
                let dto = entity |> ExerciseTemplateDto.fromDomain
                let! result = collection.ReplaceOneAsync((fun d -> d.Id = dto.Id), dto)

                if result.ModifiedCount = 0 then
                    return PersistenceError.notFound entityWithIdMsg |> Error
                else
                    return Ok()
            with
            | ex -> return PersistenceError.fromException $"update %s{entityWithIdMsg}" ex
        }

    let delete (collection: IMongoCollection<ExerciseTemplateDto>) (id: ExerciseTemplateId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! _ = collection.DeleteOneAsync(fun d -> d.Id = id)
                return Ok()
            with
            | ex -> return PersistenceError.fromException $"delete %s{entityWithIdMsg}" ex
        }

    let createRepository (collection: IMongoCollection<ExerciseTemplateDto>) =
        { new IExerciseTemplateRepository with
            member _.Create = fun entity -> create collection entity
            member _.GetById = fun id -> getById collection id
            member _.Update = fun entity -> update collection entity
            member _.Delete = fun id -> delete collection id }
