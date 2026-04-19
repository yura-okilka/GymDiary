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

module GetExerciseCategoryWorkflow =

    type public Query = { Id: string; OwnerId: string }

    type public QueryError =
        | InvalidQuery of ValidationError list
        | CategoryNotFound of id: ExerciseCategoryId * ownerId: UserId

    type public Handler(entityIdProvider: IEntityIdProvider, categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
        member _.Handle(query: Query) =
            asyncResult {
                let! categoryId, ownerId =
                    validation {
                        let! categoryId = query.Id |> Validation.checkField (nameof query.Id) entityIdProvider.TryParseResult
                        and! ownerId = query.OwnerId |> Validation.checkField (nameof query.OwnerId) entityIdProvider.TryParseResult
                        return (categoryId, ownerId)
                    }
                    |> Result.mapError InvalidQuery

                let! category =
                    categoryRepository.GetOneByOwner categoryId ownerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (CategoryNotFound(categoryId, ownerId))

                logger.LogInformation("Exercise category with id {id} was retrieved", category.Id.Value)

                return category
            }
            |> Async.StartAsTask

        interface IRequestHandler<Query, ExerciseCategory, QueryError> with
            member h.Handle query = h.Handle query

module GetExerciseCategoryResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<ExerciseCategory, GetExerciseCategoryWorkflow.QueryError>,
            onOk: Func<_, _>,
            onInvalidQuery: Func<_, _>,
            onCategoryNotFound: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | GetExerciseCategoryWorkflow.InvalidQuery e -> onInvalidQuery.Invoke(e)
            | GetExerciseCategoryWorkflow.CategoryNotFound(id, ownerId) -> onCategoryNotFound.Invoke((id, ownerId))
