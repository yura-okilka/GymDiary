namespace GymDiary.Application.ExerciseCategories

open System
open System.Threading.Tasks
open FSharp.UMX
open Microsoft.Extensions.Logging
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Users

module GetAllExerciseCategoriesWorkflow =

    type public Query = { OwnerId: Guid }

    type public Handler(categoryRepository: IExerciseCategoryRepository, logger: ILogger<Handler>) =
        member _.Handle(query: Query) : Task<ExerciseCategory list> =
            async {
                let ownerId: UserId = %query.OwnerId

                let! categories = categoryRepository.GetAllByOwner ownerId

                logger.LogInformation("Retrieved {count} exercise categories for owner {ownerId}", List.length categories, string ownerId)

                return categories
            }
            |> Async.StartAsTask
