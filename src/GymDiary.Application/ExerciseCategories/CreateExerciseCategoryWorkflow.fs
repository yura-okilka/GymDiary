module GymDiary.Application.ExerciseCategories.CreateExerciseCategoryWorkflow

open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type public Command = { Name: string; OwnerId: string }

type public CommandError =
    | InvalidCommand of ValidationError list
    | CategoryAlreadyExists of ExerciseCategoryAlreadyExistsError
    | OwnerNotFound of UserNotFoundError

    static member categoryAlreadyExists name =
        Error(CategoryAlreadyExists(ExerciseCategoryAlreadyExistsError(name)))

    static member ownerNotFound id = Error(OwnerNotFound(UserNotFoundError(id)))

type public Handler
    (idProvider: IIdProvider, userRepository: IUserRepository, categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
    member _.Handle command =
        asyncResult {
            let! category =
                validation {
                    let! id = idProvider.GenerateId() |> Ok
                    and! name = command.Name |> Validation.checkField (nameof command.Name) String50.create
                    and! ownerId = command.OwnerId |> Validation.checkField (nameof command.OwnerId) idProvider.TryParseResult
                    return ExerciseCategory.create id name ownerId
                }
                |> Result.mapError InvalidCommand

            let! ownerExists = userRepository.ExistWithId category.OwnerId

            if not ownerExists then
                return! CommandError.ownerNotFound category.OwnerId.Value

            let! categoryExists = categoryRepository.ExistWithName category.Name category.OwnerId

            if categoryExists then
                return! CommandError.categoryAlreadyExists category.Name.Value

            do! categoryRepository.Create category

            logger.LogInformation("Exercise category was created with id '{id}'", category.Id.Value)

            return category.Id.Value
        }
        |> Async.StartAsTask

    interface IRequestHandler<Command, string, CommandError> with
        member this.Handle command = this.Handle command

module ResultExtensions =
    open System

    [<Extension>]
    let Match
        (
            result: Result<string, CommandError>,
            onOk: Func<_, _>,
            onInvalidCommand: Func<_, _>,
            onCategoryAlreadyExists: Func<_, _>,
            onOwnerNotFound: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | InvalidCommand e -> onInvalidCommand.Invoke(e)
            | CategoryAlreadyExists e -> onCategoryAlreadyExists.Invoke(e)
            | OwnerNotFound e -> onOwnerNotFound.Invoke(e)
