module GymDiary.Api.Oxpecker.Router

open Oxpecker
open Oxpecker.OpenApi
open GymDiary.Api.Oxpecker.Handlers
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

let webApp = [
    GET [
        route "/ping" (text "pong")
        |> configureEndpoint _.WithTags("Ping")
        |> addOpenApiSimple<unit, string>
    ]

    subRoute "/v1/exercise-categories" [
        GET [
            route "/" <| ExerciseCategoryHandlers.getAllCategories
            routef "/{%s}" <| ExerciseCategoryHandlers.getCategoryById
        ]
        POST [
            route "/" <| ExerciseCategoryHandlers.createCategory
            |> ExerciseCategoryHandlers.createCategoryOpenApi
        ]
        PUT [ routef "/{%s}" <| ExerciseCategoryHandlers.renameCategory ]
        DELETE [ routef "/{%s}" <| ExerciseCategoryHandlers.deleteCategory ]
    ]

    subRoute "/v1/exercise-definitions" [
        POST [
            route "/" <| ExerciseDefinitionHandlers.createDefinition
            |> ExerciseDefinitionHandlers.createDefinitionOpenApi
        ]
    ]
]
