namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Persistence.Contracts
open GymDiary.Persistence.Dtos
open GymDiary.Persistence.Conversion

open MongoDB.Driver

module ExerciseCategoryRepository =

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            let operation = "create ExerciseCategory"

            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                do! collection.InsertOneAsync(dto)
                return dto.Id |> ExerciseCategoryId |> Ok
            with
            | :? MongoException as ex -> return Error(Database(operation, ex))
            | ex -> return Error(Other(operation, ex))
        }

    let findById (collection: IMongoCollection<ExerciseCategoryDto>) (ExerciseCategoryId id) =
        task {
            let operation = $"find ExerciseCategory by id '%s{id}'"

            try
                let! cursor = collection.FindAsync(fun x -> x.Id = id)
                let! dto = cursor.SingleAsync()

                return
                    dto
                    |> ExerciseCategoryDto.toDomain
                    |> Result.mapError (fun e -> DtoConversion("ExerciseCategoryDto", e))
            with
            | :? MongoException as ex -> return Error(Database(operation, ex))
            | ex -> return Error(Other(operation, ex))
        }

    let compose (collection: IMongoCollection<ExerciseCategoryDto>) =
        { new IExerciseCategoryRepository with
            member _.Create entity = create collection entity
            member _.FindById id = findById collection id }
