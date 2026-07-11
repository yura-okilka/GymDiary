namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Users

module DeleteExerciseCategoryWorkflow =

    type public Command = { Id: Guid; OwnerId: Guid }

    type public CommandError = CategoryNotFound of id: ExerciseCategoryId * ownerId: UserId

    type public Handler(categoryRepository: IExerciseCategoryRepository) =
        member _.Handle command =
            asyncResult {
                let categoryId: ExerciseCategoryId = %command.Id
                let ownerId: UserId = %command.OwnerId

                // Owner-scoped delete: removes the category only if it belongs to the caller, in a single
                // atomic operation, and reports whether anything matched so a missing/foreign id is a 404.
                let! deleted = categoryRepository.DeleteByOwner categoryId ownerId

                if not deleted then
                    return! Error(CategoryNotFound(categoryId, ownerId))
            }
            |> Async.StartAsTask

module DeleteExerciseCategoryResultExtensions =

    [<Extension>]
    let Match (result: Result<unit, DeleteExerciseCategoryWorkflow.CommandError>, onOk: Func<_, _>, onCategoryNotFound: Func<_, _>) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | DeleteExerciseCategoryWorkflow.CategoryNotFound(id, ownerId) -> onCategoryNotFound.Invoke((id, ownerId))
