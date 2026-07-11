namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open FSharp.UMX
open Microsoft.Extensions.Logging
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Users

module GetExerciseCategoryWorkflow =

    type public Query = { Id: Guid; OwnerId: Guid }

    type public QueryError = CategoryNotFound of id: ExerciseCategoryId * ownerId: UserId

    type public Handler(categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
        member _.Handle(query: Query) =
            asyncResult {
                let categoryId: ExerciseCategoryId = %query.Id
                let ownerId: UserId = %query.OwnerId

                let! category =
                    categoryRepository.GetOneByOwner categoryId ownerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (CategoryNotFound(categoryId, ownerId))

                logger.LogInformation("Exercise category with id {id} was retrieved", string category.Id)

                return category
            }
            |> Async.StartAsTask

        interface IRequestHandler<Query, ExerciseCategory, QueryError> with
            member h.Handle query = h.Handle query

module GetExerciseCategoryResultExtensions =

    [<Extension>]
    let Match (result: Result<ExerciseCategory, GetExerciseCategoryWorkflow.QueryError>, onOk: Func<_, _>, onCategoryNotFound: Func<_, _>) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | GetExerciseCategoryWorkflow.CategoryNotFound(id, ownerId) -> onCategoryNotFound.Invoke((id, ownerId))
