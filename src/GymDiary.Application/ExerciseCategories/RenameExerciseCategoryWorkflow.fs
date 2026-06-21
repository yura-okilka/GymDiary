namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging
open GymDiary.Application.Time
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

module RenameExerciseCategoryWorkflow =

    type public Command = { Id: string; OwnerId: string; Name: string }

    type public CommandError =
        | InvalidCommand of ValidationError list
        | CategoryNotFound of id: ExerciseCategoryId * ownerId: UserId
        | CategoryAlreadyExists of name: String50

    type public Handler
        (
            timeProvider: TimeProvider,
            categoryRepository: IExerciseCategoryRepository,
            logger: ILogger<Handler>
        ) =
        member _.Handle command =
            asyncResult {
                let! categoryId, ownerId, name =
                    validation {
                        let! (categoryId: ExerciseCategoryId) = command.Id |> Validation.checkField (nameof command.Id) EntityId.parse
                        and! (ownerId: UserId) = command.OwnerId |> Validation.checkField (nameof command.OwnerId) EntityId.parse
                        and! name = command.Name |> Validation.checkField (nameof command.Name) String50.create
                        return (categoryId, ownerId, name)
                    }
                    |> Result.mapError InvalidCommand

                let! category =
                    categoryRepository.GetOneByOwner categoryId ownerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (CategoryNotFound(categoryId, ownerId))

                // Only enforce name uniqueness when the name actually changes — otherwise the category's
                // own current name registers as a conflict (false positive when renaming to the same name).
                let! nameAlreadyUsed =
                    if name = category.Name then
                        Task.FromResult false
                    else
                        categoryRepository.ExistsWithName name ownerId

                if nameAlreadyUsed then
                    return! Error(CategoryAlreadyExists name)

                let renamedCategory = ExerciseCategory.rename name category timeProvider.UtcNow

                do!
                    categoryRepository.Update renamedCategory
                    |> Async.AwaitTask
                    |> AsyncResult.mapError (fun (EntityNotFound _) ->
                        CategoryNotFound(renamedCategory.Id, renamedCategory.OwnerId))

                logger.LogInformation(
                    "Exercise category with id {id} was renamed to {name}",
                    string renamedCategory.Id,
                    name.Value
                )
            }
            |> Async.StartAsTask

        interface IRequestHandler<Command, unit, CommandError> with
            member h.Handle command = h.Handle command

module RenameExerciseCategoryResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<unit, RenameExerciseCategoryWorkflow.CommandError>,
            onOk: Func<_, _>,
            onInvalidCommand: Func<_, _>,
            onCategoryNotFound: Func<_, _>,
            onCategoryAlreadyExists: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | RenameExerciseCategoryWorkflow.InvalidCommand e -> onInvalidCommand.Invoke(e)
            | RenameExerciseCategoryWorkflow.CategoryNotFound(id, ownerId) -> onCategoryNotFound.Invoke((id, ownerId))
            | RenameExerciseCategoryWorkflow.CategoryAlreadyExists e -> onCategoryAlreadyExists.Invoke(e)
