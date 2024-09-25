module GymDiary.Core.Workflows.ExerciseCategory.GetAllExerciseCategories

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Core.Workflows
open Microsoft.Extensions.Logging

type Query = { OwnerId: string }

type ExerciseCategoryDto = {
    Id: string
    Name: string
    OwnerId: string
}

type QueryResult = ExerciseCategoryDto list

type QueryError = InvalidQuery of ValidationError

type IQueryHandler = IRequestHandler<Query, QueryResult, QueryError>

type QueryHandler(categoryRepository: IExerciseCategoryRepository, logger: ILogger) =
    interface IQueryHandler with

        member _.Handle query =
            asyncResult {
                let! ownerId = Id.tryCreate (nameof query.OwnerId) query.OwnerId |> Result.mapError InvalidQuery

                let! categories = categoryRepository.GetAll ownerId

                return
                    categories
                    |> List.map (fun category -> {
                        Id = category.Id |> Id.value
                        Name = category.Name |> String50.value
                        OwnerId = category.OwnerId |> Id.value
                    })
            }
            |> Async.StartAsTask
