module GymDiary.Core.Workflows.ExerciseCategory.DeleteExerciseCategory

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Core.Workflows
open Microsoft.Extensions.Logging

type Command = { Id: string; OwnerId: string }

type CommandError =
    | InvalidCommand of ValidationError list

    static member toString error =
        match error with
        | InvalidCommand es -> es |> ValidationErrors.toString

type ICommandHandler = IRequestHandler<Command, unit, CommandError>

type CommandHandler(categoryRepository: IExerciseCategoryRepository, logger: ILogger) =
    interface ICommandHandler with
        member _.Handle command =
            asyncResult {
                let! categoryId, ownerId =
                    validation {
                        let! categoryId = Id.tryCreate (nameof command.Id) command.Id
                        and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                        return (categoryId, ownerId)
                    }
                    |> Result.mapError InvalidCommand

                // TODO: ensure it can be deleted.
                do! categoryRepository.Delete categoryId

                logger.LogInformation("Exercise category with id {id} was deleted", command.Id)
            }
            |> Async.StartAsTask
