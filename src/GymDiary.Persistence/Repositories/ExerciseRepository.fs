namespace GymDiary.Persistence.Repositories

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling

type ExerciseRepository(repository: IMongoDocumentRepository<ExerciseDocument>) =
    interface IExerciseRepository with

        member _.Create entity = async {
            let! createdDocument = entity |> ExerciseDocument.fromDomain |> repository.InsertOne

            return
                createdDocument.Id
                |> Id.create<Exercise> (nameof createdDocument.Id)
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseId>.Name, error)))
        }

        member _.GetById (exerciseId: ExerciseId) (ownerId: SportsmanId) = async {
            let exerciseId = exerciseId |> Id.value
            let ownerId = ownerId |> Id.value

            let! documentOption = repository.FindSingle(Expr.Quote(fun d -> d.Id = exerciseId && d.OwnerId = ownerId))

            return
                documentOption
                |> Option.traverseResult ExerciseDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseDocument>.Name, error)))
        }

        member _.Update entity = asyncResult {
            let id = entity.Id |> Id.value

            let! result = entity |> ExerciseDocument.fromDomain |> repository.ReplaceOne(Expr.Quote(fun d -> d.Id = id))

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<Exercise>.Name, id) |> Error
        }

        member _.Delete exerciseId = async {
            let exerciseId = exerciseId |> Id.value

            let! _ = repository.DeleteOne(Expr.Quote(fun d -> d.Id = exerciseId))
            return ()
        }
