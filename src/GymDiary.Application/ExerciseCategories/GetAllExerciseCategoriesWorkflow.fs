namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging
open GymDiary.Application.Persistence
open GymDiary.Application.Workflows
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories

module GetAllExerciseCategoriesWorkflow =

    type public Query = { OwnerId: string }

    type public QueryError = InvalidQuery of ValidationError list

    type public Handler(entityIdFactory: IEntityIdFactory, categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
        member _.Handle(query: Query) =
            asyncResult {
                let! ownerId =
                    validation {
                        let! ownerId = query.OwnerId |> Validation.checkField (nameof query.OwnerId) entityIdFactory.TryParseResult
                        return ownerId
                    }
                    |> Result.mapError InvalidQuery

                let! categories = categoryRepository.GetAllByOwner ownerId |> Async.AwaitTask

                logger.LogInformation(
                    "Retrieved {count} exercise categories for owner {ownerId}",
                    List.length categories,
                    ownerId.Value
                )

                return categories
            }
            |> Async.StartAsTask

        interface IRequestHandler<Query, ExerciseCategory list, QueryError> with
            member h.Handle query = h.Handle query

module GetAllExerciseCategoriesResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<ExerciseCategory list, GetAllExerciseCategoriesWorkflow.QueryError>,
            onOk: Func<_, _>,
            onInvalidQuery: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | GetAllExerciseCategoriesWorkflow.InvalidQuery e -> onInvalidQuery.Invoke(e)
