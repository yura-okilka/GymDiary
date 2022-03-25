namespace GymDiary.Persistence.Services

open System.Threading.Tasks

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos
open GymDiary.Persistence.Errors

open MongoDB.Driver

type IExerciseCategoryService =
    abstract member Create: ExerciseCategory -> Task<Result<ExerciseCategoryId, PersistenceError>>
    abstract member FindById: ExerciseCategoryId -> Task<Result<ExerciseCategory, PersistenceError>>

module ExerciseCategoryService =

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                do! collection.InsertOneAsync(dto)
                return dto.Id |> ExerciseCategoryId |> Ok
            with
            | :? MongoException as ex -> return ex |> Mongo |> Error
            | ex -> return ex |> Other |> Error
        }

    let findById (collection: IMongoCollection<ExerciseCategoryDto>) (ExerciseCategoryId id) =
        task {
            try
                let! cursor = collection.FindAsync(fun x -> x.Id = id)
                let! dto = cursor.SingleAsync()
                return dto |> ExerciseCategoryDto.toDomain |> Result.mapError Validation
            with
            | :? MongoException as ex -> return ex |> Mongo |> Error
            | ex -> return ex |> Other |> Error
        }

    let compose (collection: IMongoCollection<ExerciseCategoryDto>) =
        { new IExerciseCategoryService with
            member _.Create entity = create collection entity
            member _.FindById id = findById collection id }
