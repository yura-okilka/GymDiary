namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain
open GymDiary.Core.Domain.ExerciseCategoryAggregate
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling
open MongoDB.Driver

type ExerciseCategoryRepository(context: IMongoContext) =
    interface IExerciseCategoryRepository with

        member _.Create category = task {
            let document = category |> ExerciseCategoryDocument.fromDomain
            do! context.ExerciseCategories.InsertOneAsync(document)
        }

        member _.Update category = taskResult {
            let id = category.Id |> Id.value
            let document = category |> ExerciseCategoryDocument.fromDomain
            let! result = context.ExerciseCategories.ReplaceOneAsync((fun d -> d.Id = id), document)

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<ExerciseCategory>.Name, id) |> Error
        }

        member _.Delete id = task {
            let id = id |> Id.value
            let! _ = context.ExerciseCategories.DeleteOneAsync(fun d -> d.Id = id)
            return ()
        }

        member _.Get id ownerId = task {
            let id = id |> Id.value
            let ownerId = ownerId |> Id.value

            let! documentOption =
                context.ExerciseCategories
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

        member _.GetAll ownerId = task {
            let ownerId = ownerId |> Id.value

            let! documents =
                context.ExerciseCategories
                    .Find(fun d -> d.OwnerId = ownerId)
                    .ToListAsync()

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

        member _.ExistWithName name ownerId =
            let name = name |> String50.value
            let ownerId = ownerId |> Id.value

            // Consider using case-insensitive index for large collections.
            context.ExerciseCategories
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
