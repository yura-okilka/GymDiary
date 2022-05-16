namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Conversion

open FSharpx.Collections

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseCategoryRepository =

    let private unwrapId id =
        let rawId = ExerciseCategoryId.value id
        (rawId, $"ExerciseCategory with id '%s{rawId}'")

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                do! collection.InsertOneAsync(dto)

                return
                    dto.Id
                    |> ExerciseCategoryId.create (nameof dto.Id)
                    |> Result.mapError (PersistenceError.dtoConversionFailed "ExerciseCategoryId")
            with
            | ex -> return PersistenceError.fromException "create ExerciseCategory" ex
        }
        |> Async.AwaitTask

    let getAll (collection: IMongoCollection<ExerciseCategoryDto>) =
        task {
            try
                let! dtos = collection.Find(fun _ -> true).ToListAsync()

                return
                    dtos
                    |> ResizeArray.toList
                    |> List.traverseResultM ExerciseCategoryDto.toDomain
                    |> Result.mapError (PersistenceError.dtoConversionFailed "ExerciseCategoryDto")
            with
            | ex -> return PersistenceError.fromException "get all ExerciseCategories" ex
        }
        |> Async.AwaitTask

    let getById (collection: IMongoCollection<ExerciseCategoryDto>) (id: ExerciseCategoryId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! dto = collection.Find(fun d -> d.Id = id).SingleOrDefaultAsync()

                if isNull dto then
                    return PersistenceError.entityNotFoundResult entityWithIdMsg
                else
                    return
                        dto
                        |> ExerciseCategoryDto.toDomain
                        |> Result.mapError (PersistenceError.dtoConversionFailed "ExerciseCategoryDto")
            with
            | ObjectIdFormatException _ -> return PersistenceError.entityNotFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"get %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask

    let existWithName (collection: IMongoCollection<ExerciseCategoryDto>) (name: String50) =
        let name = name |> String50.value

        task {
            try
                // Consider using case insensitive index for large collections.
                let! exists = collection.Find(fun d -> d.Name.ToLower() = name.ToLower()).AnyAsync()

                return Ok(exists)
            with
            | ex -> return PersistenceError.fromException $"find ExerciseCategory with name '%s{name}'" ex
        }
        |> Async.AwaitTask

    let update (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            let _, entityWithIdMsg = unwrapId entity.Id

            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                let! result = collection.ReplaceOneAsync((fun d -> d.Id = dto.Id), dto)

                if result.ModifiedCount = 0 then
                    return PersistenceError.entityNotFoundResult entityWithIdMsg
                else
                    return Ok()
            with
            | ObjectIdFormatException _ -> return PersistenceError.entityNotFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"update %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask

    let delete (collection: IMongoCollection<ExerciseCategoryDto>) (id: ExerciseCategoryId) =
        task {
            let id, entityWithIdMsg = unwrapId id

            try
                let! result = collection.DeleteOneAsync(fun d -> d.Id = id)

                if result.DeletedCount = 0 then
                    return PersistenceError.entityNotFoundResult entityWithIdMsg
                else
                    return Ok()
            with
            | ObjectIdFormatException _ -> return PersistenceError.entityNotFoundResult entityWithIdMsg
            | ex -> return PersistenceError.fromException $"delete %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask
