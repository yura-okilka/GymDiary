module GymDiary.Api.Oxpecker.Router

open Oxpecker
open Oxpecker.OpenApi
open GymDiary.Api.Oxpecker.Handlers
open GymDiary.Core.Workflows.ExerciseCategory
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

let webApp = [
    route "/ping" <| text "pong"
    subRoute "/v1/exercise-categories" [
        GET [
            route "/" <| ExerciseCategoryHandlers.getAllCategories
            routef "/{%s}" <| ExerciseCategoryHandlers.getCategoryById
        ]
        POST [
            route "/" <| ExerciseCategoryHandlers.createCategory
            |> configureEndpoint
                _.WithSummary("Create exercise category")
                    .WithTags("Exercise Categories")
            |> addOpenApi (
                OpenApiConfig(
                    requestBody = RequestBody(typeof<ExerciseCategoryHandlers.CreateRequest>),
                    responseBodies = [|
                        ResponseBody(typeof<CreateExerciseCategory.CommandResult>, ?statusCode = Some StatusCodes.Status200OK)
                        ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status400BadRequest)
                        ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status409Conflict)
                    |]
                )
            )
        ]
    ]
]
