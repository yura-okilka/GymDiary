namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Persistence.Contracts
open GymDiary.Persistence.Dtos

open MongoDB.Driver

module ExerciseCategoryRepository =

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                do! collection.InsertOneAsync(dto)
                return dto.Id |> ExerciseCategoryId |> Ok
            with
            | :? MongoException as ex -> return Error(Database ex)
            | ex -> return Error(Other ex)
        }

    let findById (collection: IMongoCollection<ExerciseCategoryDto>) (ExerciseCategoryId id) =
        task {
            try
                let! cursor = collection.FindAsync(fun x -> x.Id = id)
                let! dto = cursor.SingleAsync()
                return dto |> ExerciseCategoryDto.toDomain |> Result.mapError Validation
            with
            | :? MongoException as ex -> return Error(Database ex)
            | ex -> return Error(Other ex)
        }

    let compose (collection: IMongoCollection<ExerciseCategoryDto>) =
        { new IExerciseCategoryRepository with
            member _.Create entity = create collection entity
            member _.FindById id = findById collection id }
