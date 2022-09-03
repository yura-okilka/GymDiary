namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetAllExerciseCategories =

    type Query = { OwnerId: string }

    type ExerciseCategoryDto =
        {
            Id: string
            Name: string
            OwnerId: string
        }

    type QueryResult = ExerciseCategoryDto list

    type QueryError = InvalidQuery of ValidationError

    type Workflow = Workflow<Query, QueryResult, QueryError>

    let execute (getAllCategoriesFromDB: SportsmanId -> Async<ExerciseCategory list>) (query: Query) =
        asyncResult {
            let! ownerId = Id.create (nameof query.OwnerId) query.OwnerId |> Result.mapError InvalidQuery

            let! categories = getAllCategoriesFromDB ownerId

            return
                categories
                |> List.map (fun category ->
                    {
                        Id = category.Id |> Id.value
                        Name = category.Name |> String50.value
                        OwnerId = category.OwnerId |> Id.value
                    })
        }
