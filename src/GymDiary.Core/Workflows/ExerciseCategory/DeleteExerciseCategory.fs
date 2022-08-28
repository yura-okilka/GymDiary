namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator
open GymDiary.Core.Persistence

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module DeleteExerciseCategory =

    type Command = { Id: string; OwnerId: string }

    type CommandError =
        | InvalidCommand of ValidationError list
        | CategoryNotFound of ExerciseCategoryNotFoundError

        static member categoryNotFound id ownerId =
            ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

        static member toString error =
            match error with
            | InvalidCommand es -> es |> List.map ValidationError.toString |> String.concat " "
            | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingContext =
        {
            ErrorEventId = Events.ExerciseCategoryDeletionFailed
            GetErrorMessage = CommandError.toString
            GetRequestInfo =
                fun cmd ->
                    Map [ (nameof cmd.Id, cmd.Id)
                          (nameof cmd.OwnerId, cmd.OwnerId) ]
        }

    let runWorkflow
        (deleteCategoryFromDB: ExerciseCategoryId -> ModifyEntityResult)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! (categoryId, ownerId) =
                validation {
                    let! categoryId = Id.create (nameof command.Id) command.Id
                    and! ownerId = Id.create (nameof command.OwnerId) command.OwnerId
                    return (categoryId, ownerId)
                }
                |> Result.mapError InvalidCommand

            do! // TODO: ensure it can be deleted.
                deleteCategoryFromDB categoryId
                |> AsyncResult.mapError (function
                    | EntityNotFound _ -> CommandError.categoryNotFound categoryId ownerId)

            logger.LogInformation(
                Events.ExerciseCategoryDeleted,
                "Exercise category with id '{id}' was deleted",
                command.Id
            )
        }
