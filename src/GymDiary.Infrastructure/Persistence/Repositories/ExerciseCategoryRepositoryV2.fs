namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open FsToolkit.ErrorHandling
open MongoDB.Driver

type ExerciseCategoryRepositoryV2(context: IMongoContext) =
    interface IExerciseCategoryRepository with

        member _.Create category = task {
            let document = category |> ExerciseCategoryDocumentV2.fromDomain
            do! context.ExerciseCategoriesV2.InsertOneAsync(document)
        }

        member _.Update category = taskResult {
            let document = category |> ExerciseCategoryDocumentV2.fromDomain
            let! result = context.ExerciseCategoriesV2.ReplaceOneAsync((fun d -> d.Id = category.Id.Value), document)

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<ExerciseCategory>.Name, category.Id.Value) |> Error
        }

        member _.Delete id = task {
            let id = id.Value
            let! _ = context.ExerciseCategoriesV2.DeleteOneAsync(fun d -> d.Id = id)
            return ()
        }

        member _.Get id ownerId = task {
            let id = id.Value
            let ownerId = ownerId.Value

            let! documentOption =
                context.ExerciseCategoriesV2
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult ExerciseCategoryDocumentV2.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionExceptionV2(typeof<ExerciseCategoryDocumentV2>.Name, error)))
        }

        member _.GetAll ownerId = task {
            let ownerId = ownerId.Value

            let! documents =
                context.ExerciseCategoriesV2
                    .Find(fun d -> d.OwnerId = ownerId)
                    .ToListAsync()

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM ExerciseCategoryDocumentV2.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionExceptionV2(typeof<ExerciseCategoryDocumentV2>.Name, error)))
        }

        member _.ExistWithName name ownerId =
            let name = name.Value
            let ownerId = ownerId.Value

            // Consider using case-insensitive index for large collections.
            context.ExerciseCategoriesV2
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
