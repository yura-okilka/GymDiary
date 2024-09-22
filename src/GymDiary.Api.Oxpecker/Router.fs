module GymDiary.Api.Oxpecker.Router

open Oxpecker
open GymDiary.Api.Oxpecker.Handlers
open type Microsoft.AspNetCore.Http.TypedResults

let webApp = [
    route "/ping" <| text "pong"
    subRoute "/v1/exercise-categories" [
        GET [
            route "/" <| ExerciseCategoryHandlers.getAllCategories
            routef "/{%s}" <| ExerciseCategoryHandlers.getCategoryById
        ]
    ]
]
