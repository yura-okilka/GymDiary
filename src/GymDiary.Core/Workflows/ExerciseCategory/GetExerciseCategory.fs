namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetExerciseCategory =

    type Query = { Id: string; OwnerId: string }

    type QueryResult =
        {
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
            | InvalidQuery es -> es |> List.map ValidationError.toString |> String.concat " "
            | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString

    type Workflow = Workflow<Query, QueryResult, QueryError>

    let runWorkflow
        (getCategoryByIdFromDB: SportsmanId -> ExerciseCategoryId -> Async<ExerciseCategory option>)
        (query: Query)
        =
        asyncResult {
            let! (categoryId, ownerId) =
                validation {
                    let! categoryId = Id.create (nameof query.Id) query.Id
                    and! ownerId = Id.create (nameof query.OwnerId) query.OwnerId
                    return (categoryId, ownerId)
                }
                |> Result.mapError InvalidQuery

            let! category =
                getCategoryByIdFromDB ownerId categoryId
                |> AsyncResult.requireSome (QueryError.categoryNotFound categoryId ownerId)

            return
                {
                    Id = category.Id |> Id.value
                    Name = category.Name |> String50.value
                    OwnerId = category.OwnerId |> Id.value
                }
        }
