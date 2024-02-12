namespace GymDiary.Api

open Giraffe

open GymDiary.Api.DependencyInjection
open GymDiary.Api.JsonBinding
open GymDiary.Api.HttpHandlers

open Microsoft.AspNetCore.Http

module Router =

    let bindJsonSafe<'T> = tryBindJson<'T> ErrorHandlers.parsingError

    let webApp (root: CompositionRoot) : (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [
            POST >=> routef "/v1/sportsmen/%s/exerciseCategories" (fun sportsmanId -> bindJsonSafe (ExerciseCategoryHandlers.create root.CreateExerciseCategory sportsmanId))
            GET >=> routef "/v1/sportsmen/%s/exerciseCategories" (ExerciseCategoryHandlers.getAll root.GetAllExerciseCategories)
            GET >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.getById root.GetExerciseCategory)
            PUT >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (fun ids -> bindJsonSafe (ExerciseCategoryHandlers.rename root.RenameExerciseCategory ids))
            DELETE >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.delete root.DeleteExerciseCategory)

            POST >=> routef "/v1/sportsmen/%s/exercises" (fun sportsmanId -> bindJsonSafe (ExerciseHandlers.create root.CreateExercise sportsmanId))

            route "/ping" >=> noResponseCaching >=> text "pong"
            RequestErrors.NOT_FOUND "Not Found"
        ]
