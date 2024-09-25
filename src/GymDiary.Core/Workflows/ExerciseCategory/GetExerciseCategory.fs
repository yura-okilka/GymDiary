module GymDiary.Core.Workflows.ExerciseCategory.GetExerciseCategory

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Core.Workflows
open Microsoft.Extensions.Logging

type Query = { Id: string; OwnerId: string }

type QueryResult = {
    Id: string
    Name: string
    OwnerId: string
}

type QueryError =
    | InvalidQuery of ValidationError list
    | CategoryNotFound of ExerciseCategoryNotFoundError

    static member categoryNotFound id ownerId =
        ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

    static member toString error =
        match error with
        | InvalidQuery es -> es |> ValidationErrors.toString
        | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString

type IQueryHandler = IRequestHandler<Query, QueryResult, QueryError>

type QueryHandler(categoryRepository: IExerciseCategoryRepository, logger: ILogger) =
    interface IQueryHandler with

        member _.Handle query =
            asyncResult {
                let! categoryId, ownerId =
                    validation {
                        let! categoryId = Id.tryCreate (nameof query.Id) query.Id
                        and! ownerId = Id.tryCreate (nameof query.OwnerId) query.OwnerId
                        return (categoryId, ownerId)
                    }
                    |> Result.mapError InvalidQuery

                let! category =
                    categoryRepository.Get categoryId ownerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (QueryError.categoryNotFound categoryId ownerId)

                return {
                    Id = category.Id |> Id.value
                    Name = category.Name |> String50.value
                    OwnerId = category.OwnerId |> Id.value
                }
            }
            |> Async.StartAsTask
