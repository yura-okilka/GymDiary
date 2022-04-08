namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Persistence.Contracts
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Dtos
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseCategoryRepository =

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        task {
            try
                let dto = entity |> ExerciseCategoryDto.fromDomain
                do! collection.InsertOneAsync(dto)

                return
                    dto.Id
                    |> ExerciseCategoryId.create (nameof dto.Id)
                    |> Result.mapError (fun e -> DtoConversion("ExerciseCategoryId", e))
            with
            | ex -> return PersistenceError.createResult "create ExerciseCategory" ex
        }

    let findById (collection: IMongoCollection<ExerciseCategoryDto>) (id: ExerciseCategoryId) =
        task {
            let id = ExerciseCategoryId.value id
            let operation = $"find ExerciseCategory by id '%s{id}'"

            try
                let! cursor = collection.FindAsync(fun x -> x.Id = id)
                let! dto = cursor.SingleOrDefaultAsync()

                return
                    dto
                    |> Option.ofRecord
                    |> Option.traverseResult ExerciseCategoryDto.toDomain
                    |> Result.mapError (fun e -> DtoConversion("ExerciseCategoryDto", e))
            with
            | ex -> return PersistenceError.createResult operation ex
        }

    let compose (collection: IMongoCollection<ExerciseCategoryDto>) =
        { new IExerciseCategoryRepository with
            member _.Create entity = create collection entity
            member _.FindById id = findById collection id }
