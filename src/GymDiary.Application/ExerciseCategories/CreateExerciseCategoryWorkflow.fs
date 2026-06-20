namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open GymDiary.Domain.Users
open Microsoft.Extensions.Logging
open GymDiary.Application.Time
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes

module CreateExerciseCategoryWorkflow =

    type public Command = { Name: string; OwnerId: string }

    type public CommandError =
        | InvalidCommand of ValidationError list
        | CategoryAlreadyExists of name: String50
        | OwnerNotFound of id: UserId

        static member categoryAlreadyExists name = Error(CategoryAlreadyExists(name))

        static member ownerNotFound id = Error(OwnerNotFound(id))

    type public Handler
        (
            timeProvider: TimeProvider,
            userRepository: IUserRepository,
            categoryRepository: IExerciseCategoryRepository,
            logger: ILogger<Handler>
        ) =
        member _.Handle command =
            asyncResult {
                let id: ExerciseCategoryId = EntityId.create ()

                let! category =
                    validation {
                        let! name = command.Name |> Validation.checkField (nameof command.Name) String50.create
                        and! (ownerId: UserId) = command.OwnerId |> Validation.checkField (nameof command.OwnerId) EntityId.parse
                        return ExerciseCategory.create id name ownerId timeProvider.UtcNow
                    }
                    |> Result.mapError InvalidCommand

                let! ownerExists = userRepository.ExistsWithId category.OwnerId

                if not ownerExists then
                    return! CommandError.ownerNotFound category.OwnerId

                let! categoryExists = categoryRepository.ExistsWithName category.Name category.OwnerId

                if categoryExists then
                    return! CommandError.categoryAlreadyExists category.Name

                do! categoryRepository.Create category

                logger.LogInformation("Exercise category was created with id {id}", EntityId.toString category.Id)

                return EntityId.toString category.Id
            }
            |> Async.StartAsTask

        interface IRequestHandler<Command, string, CommandError> with
            member h.Handle command = h.Handle command

module CreateExerciseCategoryResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<string, CreateExerciseCategoryWorkflow.CommandError>,
            onOk: Func<_, _>,
            onInvalidCommand: Func<_, _>,
            onCategoryAlreadyExists: Func<_, _>,
            onOwnerNotFound: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | CreateExerciseCategoryWorkflow.InvalidCommand e -> onInvalidCommand.Invoke(e)
            | CreateExerciseCategoryWorkflow.CategoryAlreadyExists e -> onCategoryAlreadyExists.Invoke(e)
            | CreateExerciseCategoryWorkflow.OwnerNotFound e -> onOwnerNotFound.Invoke(e)
