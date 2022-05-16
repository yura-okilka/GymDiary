namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetAllExerciseCategories =

    type Query = { OwnerId: string }

    type ExerciseCategoryDto =
        { Id: string
          Name: string
          OwnerId: string }

    type QueryResult = ExerciseCategoryDto list

    type QueryError =
        | Validation of ValidationError
        | Persistence of PersistenceError

        static member validation e = Validation e
        static member persistence e = Persistence e

    type Workflow = Workflow<Query, QueryResult, QueryError>

    let createWorkflow
        (getAllCategoriesFromDB: SportsmanId -> Async<Result<ExerciseCategory list, PersistenceError>>)
        : Workflow =
        fun query ->
            asyncResult {
                let! ownerId =
                    SportsmanId.create (nameof query.OwnerId) query.OwnerId |> Result.mapError QueryError.validation

                let! categories = getAllCategoriesFromDB ownerId |> AsyncResult.mapError QueryError.persistence

                return
                    categories
                    |> List.map (fun category ->
                        { Id = category.Id |> ExerciseCategoryId.value
                          Name = category.Name |> String50.value
                          OwnerId = category.OwnerId |> SportsmanId.value })
            }
