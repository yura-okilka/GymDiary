namespace GymDiary.Persistence.Repositories

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling

type ExerciseCategoryRepository(repository: IMongoRepository<ExerciseCategoryDocument>) =
    interface IExerciseCategoryRepository with

        member _.Create entity = async {
            let! createdDocument = entity |> ExerciseCategoryDocument.fromDomain |> repository.InsertOne

            return
                createdDocument.Id
                |> Id.create<ExerciseCategory> (nameof createdDocument.Id)
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseCategoryId>.Name, error)))
        }

        member _.GetAll ownerId = async {
            let ownerId = ownerId |> Id.value
            let! documents = repository.FindAll(Expr.Quote(fun d -> d.OwnerId = ownerId))

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

        member _.GetById (categoryId: ExerciseCategoryId) (ownerId: SportsmanId) = async {
            let categoryId = categoryId |> Id.value
            let ownerId = ownerId |> Id.value

            let! documentOption = repository.FindSingle(Expr.Quote(fun d -> d.Id = categoryId && d.OwnerId = ownerId))

            return
                documentOption
                |> Option.traverseResult ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

        member _.ExistWithName (name: String50) (ownerId: SportsmanId) =
            let name = name |> String50.value
            let ownerId = ownerId |> Id.value

            // Consider using case insensitive index for large collections.
            repository.Any(Expr.Quote(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId))

        member _.Update entity = asyncResult {
            let id = entity.Id |> Id.value

            let! result =
                entity
                |> ExerciseCategoryDocument.fromDomain
                |> repository.ReplaceOne(Expr.Quote(fun d -> d.Id = id))

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<ExerciseCategory>.Name, id) |> Error
        }

        member _.Delete categoryId = asyncResult {
            let categoryId = categoryId |> Id.value

            let! result = repository.DeleteOne(Expr.Quote(fun d -> d.Id = categoryId))

            if result.DeletedCount = 0 then
                return! EntityNotFound(typeof<ExerciseCategory>.Name, categoryId) |> Error
        }
