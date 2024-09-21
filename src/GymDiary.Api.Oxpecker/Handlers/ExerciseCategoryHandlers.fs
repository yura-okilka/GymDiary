module GymDiary.Api.Oxpecker.Handlers.ExerciseCategoryHandlers

open Oxpecker
open GymDiary.Core.Workflows.ExerciseCategory
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

let getAllCategories: EndpointHandler =
    fun (ctx: HttpContext) -> task {
        let handler = ctx.GetService<GetAllExerciseCategories.IQueryHandler>()

        let! result = handler.Handle({ OwnerId = "65e8edad477943d2b3844853" }) |> Async.StartAsTask

        match result with
        | Ok data -> return! ctx.Write <| Ok data
        | Error(GetAllExerciseCategories.InvalidQuery e) -> return! ctx.Write <| BadRequest "Responses.validationError e"
    }
