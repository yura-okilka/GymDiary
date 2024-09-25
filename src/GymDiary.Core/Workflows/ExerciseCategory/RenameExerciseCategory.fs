module GymDiary.Core.Workflows.ExerciseCategory.RenameExerciseCategory

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Persistence
open Microsoft.Extensions.Logging

type Command = {
    Id: string
    OwnerId: string
    Name: string
}

type CommandError =
    | InvalidCommand of ValidationError list
    | CategoryNotFound of ExerciseCategoryNotFoundError
    | NameAlreadyUsed of ExerciseCategoryAlreadyExistsError

    static member categoryNotFound id ownerId =
        ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

    static member nameAlreadyUsed name =
        ExerciseCategoryAlreadyExistsError.create name |> NameAlreadyUsed

    static member toString error =
        match error with
        | InvalidCommand es -> es |> ValidationErrors.toString
        | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString
        | NameAlreadyUsed e -> e |> ExerciseCategoryAlreadyExistsError.toString

type ICommandHandler = IRequestHandler<Command, unit, CommandError>

type CommandHandler(categoryRepository: IExerciseCategoryRepository, logger: ILogger) =
    interface ICommandHandler with

        member _.Handle command =
            asyncResult {
                let! categoryId, ownerId, name =
                    validation {
                        let! categoryId = Id.tryCreate (nameof command.Id) command.Id
                        and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                        and! name = String50.create (nameof command.Name) command.Name
                        return (categoryId, ownerId, name)
                    }
                    |> Result.mapError InvalidCommand

                let! categoryExists = categoryRepository.ExistWithName name ownerId

                if categoryExists then
                    return! CommandError.nameAlreadyUsed name |> Error

                let! category =
                    categoryRepository.Get categoryId ownerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (CommandError.categoryNotFound categoryId ownerId)

                let renamedCategory = category |> ExerciseCategoryAggregate.rename name

                do!
                    categoryRepository.Update renamedCategory
                    |> Async.AwaitTask
                    |> AsyncResult.mapError (function
                        | EntityNotFound _ -> CommandError.categoryNotFound renamedCategory.Id renamedCategory.OwnerId)

                logger.LogInformation("Exercise category with id '{id}' was renamed to '{name}'", command.Id, command.Name)
            }
            |> Async.StartAsTask
