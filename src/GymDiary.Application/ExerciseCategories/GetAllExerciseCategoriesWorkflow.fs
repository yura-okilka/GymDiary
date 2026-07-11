namespace GymDiary.Application.ExerciseCategories

open System
open System.Threading.Tasks
open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories

module GetAllExerciseCategoriesWorkflow =

    type public Query = { OwnerId: Guid }

    type public Handler(categoryRepository: IExerciseCategoryRepository) =
        member _.Handle(query: Query) : Task<ExerciseCategory list> =
            categoryRepository.GetAllByOwner(%query.OwnerId) |> Async.StartAsTask
