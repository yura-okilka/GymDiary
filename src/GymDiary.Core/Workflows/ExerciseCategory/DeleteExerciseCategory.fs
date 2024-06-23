namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module DeleteExerciseCategory =

    type Command = { Id: string; OwnerId: string }

    type CommandError =
        | InvalidCommand of ValidationError list

        static member toString error =
            match error with
            | InvalidCommand es -> es |> ValidationErrors.toString

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.ExerciseCategoryDeletionFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) =
                Map [ (nameof command.Id, command.Id); (nameof command.OwnerId, command.OwnerId) ]
        }

    let execute (deleteCategoryFromDB: ExerciseCategoryId -> Async<unit>) (logger: ILogger) (command: Command) = asyncResult {
        let! (categoryId, ownerId) =
            validation {
                let! categoryId = Id.tryCreate (nameof command.Id) command.Id
                and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                return (categoryId, ownerId)
            }
            |> Result.mapError InvalidCommand

        // TODO: ensure it can be deleted.
        do! deleteCategoryFromDB categoryId

        logger.LogInformation(DomainEvents.ExerciseCategoryDeleted, "Exercise category with id '{id}' was deleted", command.Id)
    }
