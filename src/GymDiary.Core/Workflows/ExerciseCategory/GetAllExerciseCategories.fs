namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetAllExerciseCategories =

    type Query = { OwnerId: string }

    type ExerciseCategoryDocument =
        { Id: string
          Name: string
          OwnerId: string }

    type QueryResult = ExerciseCategoryDocument list

    type QueryError =
        | Validation of ValidationError
        | Persistence of PersistenceError

        static member validation e = Validation e
        static member persistence e = Persistence e

    type Workflow = Workflow<Query, QueryResult, QueryError>

    let runWorkflow (getAllCategoriesFromDB: SportsmanId -> PersistenceResult<ExerciseCategory list>) (query: Query) =
        asyncResult {
            let! ownerId = Id.create (nameof query.OwnerId) query.OwnerId |> Result.mapError QueryError.validation

            let! categories = getAllCategoriesFromDB ownerId |> AsyncResult.mapError QueryError.persistence

            return
                categories
                |> List.map (fun category ->
                    { Id = category.Id |> Id.value
                      Name = category.Name |> String50.value
                      OwnerId = category.OwnerId |> Id.value })
        }
