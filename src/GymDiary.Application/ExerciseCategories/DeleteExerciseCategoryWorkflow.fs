namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Users
open GymDiary.Domain.Primitives.SharedTypes

module DeleteExerciseCategoryWorkflow =

    type public Command = { Id: string; OwnerId: string }

    type public CommandError =
        | InvalidCommand of ValidationError list
        | CategoryNotFound of id: ExerciseCategoryId * ownerId: UserId

    type public Handler(categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
        member _.Handle command =
            asyncResult {
                let! categoryId, ownerId =
                    validation {
                        let! (categoryId: ExerciseCategoryId) = command.Id |> Validation.checkField (nameof command.Id) EntityId.parse
                        and! (ownerId: UserId) = command.OwnerId |> Validation.checkField (nameof command.OwnerId) EntityId.parse
                        return (categoryId, ownerId)
                    }
                    |> Result.mapError InvalidCommand

                // Owner-scoped delete: removes the category only if it belongs to the caller, in a single
                // atomic operation, and reports whether anything matched so a missing/foreign id is a 404.
                let! deleted = categoryRepository.DeleteByOwner categoryId ownerId

                if not deleted then
                    return! Error(CategoryNotFound(categoryId, ownerId))

                logger.LogInformation("Exercise category with id {id} was deleted", EntityId.toString categoryId)
            }
            |> Async.StartAsTask

        interface IRequestHandler<Command, unit, CommandError> with
            member h.Handle command = h.Handle command

module DeleteExerciseCategoryResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<unit, DeleteExerciseCategoryWorkflow.CommandError>,
            onOk: Func<_, _>,
            onInvalidCommand: Func<_, _>,
            onCategoryNotFound: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | DeleteExerciseCategoryWorkflow.InvalidCommand e -> onInvalidCommand.Invoke(e)
            | DeleteExerciseCategoryWorkflow.CategoryNotFound(id, ownerId) -> onCategoryNotFound.Invoke((id, ownerId))
