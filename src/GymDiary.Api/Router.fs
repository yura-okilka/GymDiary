namespace GymDiary.Api

open Giraffe

open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers

open Microsoft.AspNetCore.Http

module Router =

    let webApp (root: CompositionRoot) : (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [
            subRoute "/v1/exerciseCategories"
                (choose [
                    POST >=> route "" >=> ExerciseCategoryHandlers.create root.CreateExerciseCategory
                    GET >=> route "" >=> ExerciseCategoryHandlers.getAll
                    GET >=> routef "/%s" (ExerciseCategoryHandlers.getById root.GetExerciseCategory)
                    PUT >=> routef "/%s" (ExerciseCategoryHandlers.rename root.RenameExerciseCategory)
                    DELETE >=> routef "/%s" ExerciseCategoryHandlers.delete ])
            route "/ping" >=> noResponseCaching >=> text "pong"
            RequestErrors.NOT_FOUND "Not Found" ]
            