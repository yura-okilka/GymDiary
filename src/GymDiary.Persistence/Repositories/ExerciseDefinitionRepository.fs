namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling
open MongoDB.Driver

type ExerciseDefinitionRepository(context: IMongoContext) =
    interface IExerciseDefinitionRepository with

        member _.Create entity = task {
            let document = entity |> ExerciseDefinitionDocument.fromDomain
            do! context.ExerciseDefinitions.InsertOneAsync(document)
        }

        member _.Update entity = taskResult {
            let id = entity.Id |> Id.value
            let document = entity |> ExerciseDefinitionDocument.fromDomain
            let! result = context.ExerciseDefinitions.ReplaceOneAsync((fun d -> d.Id = id), document)

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<ExerciseDefinition>.Name, id) |> Error
        }

        member _.Delete id = task {
            let id = id |> Id.value
            let! _ = context.ExerciseDefinitions.DeleteOneAsync(fun d -> d.Id = id)
            return ()
        }

        member _.Get id ownerId = task {
            let id = id |> Id.value
            let ownerId = ownerId |> Id.value

            let! documentOption =
                context.ExerciseDefinitions
                    .Find(fun d -> d.Id = id && d.OwnerId = ownerId)
                    .SingleOrNoneAsync()

            return
                documentOption
                |> Option.traverseResult ExerciseDefinitionDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseDefinitionDocument>.Name, error)))
        }
