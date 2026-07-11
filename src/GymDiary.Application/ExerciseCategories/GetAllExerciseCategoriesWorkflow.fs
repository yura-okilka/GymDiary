namespace GymDiary.Application.ExerciseCategories

open System
open System.Threading.Tasks
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Users

module GetAllExerciseCategoriesWorkflow =

    type public Query = { OwnerId: Guid }

    type public Handler(categoryRepository: IExerciseCategoryRepository) =
        member _.Handle(query: Query) : Task<ExerciseCategory list> =
            async {
                let ownerId: UserId = %query.OwnerId

                let! categories = categoryRepository.GetAllByOwner ownerId

                return categories
            }
            |> Async.StartAsTask
