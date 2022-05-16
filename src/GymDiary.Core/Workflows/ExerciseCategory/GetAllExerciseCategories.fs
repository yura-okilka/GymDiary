namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetAllExerciseCategories =

    type ExerciseCategoryDto =
        { Id: string
          Name: string
          OwnerId: string }

    type QueryResult = ExerciseCategoryDto list

    type QueryError =
        | Persistence of PersistenceError

        static member persistence e = Persistence e

    type Workflow = Workflow<unit, QueryResult, QueryError>

    let createWorkflow
        (getAllCategoriesFromDB: unit -> Async<Result<ExerciseCategory list, PersistenceError>>)
        : Workflow =
        fun _ ->
            asyncResult {
                let! categories = getAllCategoriesFromDB () |> AsyncResult.mapError QueryError.persistence

                return
                    categories
                    |> List.map (fun category ->
                        { Id = category.Id |> ExerciseCategoryId.value
                          Name = category.Name |> String50.value
                          OwnerId = category.OwnerId |> SportsmanId.value })
            }
