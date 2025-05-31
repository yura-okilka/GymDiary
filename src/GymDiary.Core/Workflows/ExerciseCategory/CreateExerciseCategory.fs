module GymDiary.Core.Workflows.ExerciseCategory.CreateExerciseCategory

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Core.Workflows
open Microsoft.Extensions.Logging

type Command = { Name: string; OwnerId: string }

type CommandError =
    | InvalidCommand of ValidationError list
    | CategoryAlreadyExists of ExerciseCategoryAlreadyExistsError
    | OwnerNotFound of OwnerNotFoundError

    static member categoryAlreadyExists name =
        ExerciseCategoryAlreadyExistsError.create name |> CategoryAlreadyExists

    static member ownerNotFound id = OwnerNotFoundError.create id |> OwnerNotFound

    static member toString error =
        match error with
        | InvalidCommand es -> es |> ValidationErrors.toString
        | CategoryAlreadyExists e -> e |> ExerciseCategoryAlreadyExistsError.toString
        | OwnerNotFound e -> e |> OwnerNotFoundError.toString

type ICommandHandler = IRequestHandler<Command, string, CommandError>

type CommandHandler
    (idProvider: IIdProvider, userRepository: IUserRepository, categoryRepository: IExerciseCategoryRepository, logger: ILogger) =
    interface ICommandHandler with
        member _.Handle command =
            asyncResult {
                let! category =
                    validation {
                        let! id = idProvider.GenerateId() |> Ok
                        and! name = String50.create (nameof command.Name) command.Name
                        and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                        return ExerciseCategoryAggregate.create id name ownerId
                    }
                    |> Result.mapError InvalidCommand

                let! ownerExists = userRepository.ExistWithId category.OwnerId

                if not ownerExists then
                    return! CommandError.ownerNotFound category.OwnerId |> Error

                let! categoryExists = categoryRepository.ExistWithName category.Name category.OwnerId

                if categoryExists then
                    return! CommandError.categoryAlreadyExists category.Name |> Error

                do! categoryRepository.Create category

                logger.LogInformation("Exercise category was created with id '{id}'", category.Id)

                return category.Id |> Id.value
            }
            |> Async.StartAsTask
