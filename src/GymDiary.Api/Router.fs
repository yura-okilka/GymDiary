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
            POST >=> routef "/v1/users/%s/exercise-categories" (fun userId -> bindJsonSafe (ExerciseCategoryHandlers.create root.CreateExerciseCategory userId))
            GET >=> routef "/v1/users/%s/exercise-categories" (ExerciseCategoryHandlers.getAll root.GetAllExerciseCategories)
            GET >=> routef "/v1/users/%s/exercise-categories/%s" (ExerciseCategoryHandlers.getById root.GetExerciseCategory)
            PUT >=> routef "/v1/users/%s/exercise-categories/%s" (fun ids -> bindJsonSafe (ExerciseCategoryHandlers.rename root.RenameExerciseCategory ids))
            DELETE >=> routef "/v1/users/%s/exercise-categories/%s" (ExerciseCategoryHandlers.delete root.DeleteExerciseCategory)

            POST >=> routef "/v1/users/%s/exercises" (fun userId -> bindJsonSafe (ExerciseHandlers.create root.CreateExercise userId))

            POST >=> route "/v1/users" >=> bindJsonSafe (UserHandlers.create root.CreateUser)

            route "/ping" >=> noResponseCaching >=> text "pong"
            RequestErrors.NOT_FOUND "Not Found"
        ]
