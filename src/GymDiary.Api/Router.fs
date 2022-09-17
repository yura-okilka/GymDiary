namespace GymDiary.Api

open Giraffe

open GymDiary.Api.DependencyInjection
open GymDiary.Api.JsonBinding
open GymDiary.Api.HttpHandlers

open Microsoft.AspNetCore.Http

module Router =

    let bindJsonSafe<'T> = tryBindJson<'T> ErrorHandler.parsingError

    let webApp (root: CompositionRoot) : (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [ POST
                 >=> routef "/v1/sportsmen/%s/exerciseCategories" (fun id ->
                     bindJsonSafe (fun req -> ExerciseCategoryHandlers.create root.CreateExerciseCategory id req))

                 GET >=> routef "/v1/sportsmen/%s/exerciseCategories" (ExerciseCategoryHandlers.getAll root.GetAllExerciseCategories)
                 GET >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.getById root.GetExerciseCategory)
                 PUT
                 >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (fun ids ->
                     bindJsonSafe (fun req -> ExerciseCategoryHandlers.rename root.RenameExerciseCategory ids req))

                 DELETE >=> routef "/v1/sportsmen/%s/exerciseCategories/%s" (ExerciseCategoryHandlers.delete root.DeleteExerciseCategory)

                 route "/ping" >=> noResponseCaching >=> text "pong"
                 RequestErrors.NOT_FOUND "Not Found" ]
