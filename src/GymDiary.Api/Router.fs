namespace GymDiary.Api

open Giraffe

open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers

open Microsoft.AspNetCore.Http

module Router =

    let webApp (root: CompositionRoot) : (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [
            subRoutef "/v1/sportsmen/%s/exerciseCategories" (fun sportsmanId ->
                (choose [
                    POST >=> route "" >=> ExerciseCategoryHandlers.create root.CreateExerciseCategory sportsmanId
                    GET >=> route "" >=> ExerciseCategoryHandlers.getAll root.GetAllExerciseCategories sportsmanId
                    GET >=> routef "/%s" (fun categoryId -> ExerciseCategoryHandlers.getById root.GetExerciseCategory sportsmanId categoryId)
                    PUT >=> routef "/%s" (fun categoryId -> ExerciseCategoryHandlers.rename root.RenameExerciseCategory sportsmanId categoryId)
                    DELETE >=> routef "/%s" (fun categoryId -> ExerciseCategoryHandlers.delete root.DeleteExerciseCategory sportsmanId categoryId) ]))
            route "/ping" >=> noResponseCaching >=> text "pong"
            RequestErrors.NOT_FOUND "Not Found" ]
