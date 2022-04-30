namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Core.Workflows.ExerciseCategory

open Microsoft.AspNetCore.Http

module ExerciseCategoryHandlers =

    let create (createExerciseCategory: CreateExerciseCategory.Workflow) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! command = ctx.BindJsonAsync<CreateExerciseCategory.Command>()
                let! result = createExerciseCategory command

                return!
                    match result with
                    | Ok data -> json data next ctx
                    | Error error -> RequestErrors.badRequest (text "Bad Request") next ctx // TODO: match errors to HTTP codes.
            }

    let getAll: HttpHandler = text "getAll"

    let getById (getExerciseCategory: GetExerciseCategory.Workflow) (id: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = getExerciseCategory { Id = id } // TODO: Use alias where possible.

                return!
                    match result with
                    | Ok data -> json data next ctx
                    | Error error -> RequestErrors.notFound (text "Not Found") next ctx // TODO: match errors to HTTP codes.
            }

    type RenameRequest = { Name: string }

    let rename (renameExerciseCategory: RenameExerciseCategory.Workflow) (id: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<RenameRequest>()
                let! result = renameExerciseCategory { Id = id; Name = request.Name }

                return!
                    match result with
                    | Ok _ -> Successful.ok (text "Ok") next ctx
                    | Error error -> RequestErrors.badRequest (text "Bad Request") next ctx // TODO: match errors to HTTP codes.
            }

    let delete (id: string) : HttpHandler = text $"delete %s{id}"
