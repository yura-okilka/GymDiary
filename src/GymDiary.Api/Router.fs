namespace GymDiary.Api

open Giraffe

open Microsoft.AspNetCore.Http

[<RequireQualifiedAccess>]
module Router =

    let webApp: (HttpFunc -> HttpContext -> HttpFuncResult) =
        choose [ subRoute
                     "/v1/exerciseCategories"
                     (choose [ POST >=> route "" >=> HttpHandlers.ExerciseCategory.create
                               GET >=> route "" >=> HttpHandlers.ExerciseCategory.getAll
                               GET >=> routef "/%s" (fun id -> id |> HttpHandlers.ExerciseCategory.getById)
                               PUT >=> routef "/%s" (fun id -> id |> HttpHandlers.ExerciseCategory.rename)
                               DELETE >=> routef "/%s" (fun id -> id |> HttpHandlers.ExerciseCategory.delete) ])
                 route "/ping" >=> HttpHandlers.pingPong
                 RequestErrors.NOT_FOUND "Not Found" ]
