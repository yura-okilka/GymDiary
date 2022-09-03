namespace GymDiary.Api

open Giraffe

open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers

open Microsoft.AspNetCore.Http

module Router =

    let webApp (root: CompositionRoot) : (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [ POST >=> routef "/v1/sportsmen/%s/exerciseCategories" (ExerciseCategoryHandlers.create root.CreateExerciseCategory)
                 GET >=> routef "/v1/sportsmen/%s/exerciseCategories" (ExerciseCategoryHandlers.getAll root.GetAllExerciseCategories)
                 GET >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.getById root.GetExerciseCategory)
                 PUT >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.rename root.RenameExerciseCategory)
                 DELETE >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.delete root.DeleteExerciseCategory)
                 route "/ping" >=> noResponseCaching >=> text "pong"
                 RequestErrors.NOT_FOUND "Not Found" ]
