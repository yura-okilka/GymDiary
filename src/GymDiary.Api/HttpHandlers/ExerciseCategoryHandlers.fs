namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Domain.Errors
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
                    | Ok data -> Successful.CREATED data next ctx

                    | Error (CreateExerciseCategory.Validation errors) ->
                        RequestErrors.BAD_REQUEST (ErrorResponse.from errors) next ctx

                    | Error (CreateExerciseCategory.Domain error) ->
                        RequestErrors.CONFLICT (ErrorResponse.from error) next ctx

                    | Error (CreateExerciseCategory.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }

    let getAll (getAllExerciseCategories: GetAllExerciseCategories.Workflow) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = getAllExerciseCategories ()

                return!
                    match result with
                    | Ok data -> Successful.OK data next ctx

                    | Error (GetAllExerciseCategories.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }

    let getById (getExerciseCategory: GetExerciseCategory.Workflow) (id: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = getExerciseCategory { Id = id }

                return!
                    match result with
                    | Ok data -> Successful.OK data next ctx

                    | Error (GetExerciseCategory.Domain (ExerciseCategoryNotFound as error)) ->
                        RequestErrors.NOT_FOUND (ErrorResponse.from error) next ctx

                    | Error (GetExerciseCategory.Domain error) ->
                        RequestErrors.CONFLICT (ErrorResponse.from error) next ctx

                    | Error (GetExerciseCategory.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }

    type RenameRequest = { Name: string }

    let rename (renameExerciseCategory: RenameExerciseCategory.Workflow) (id: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<RenameRequest>()
                let! result = renameExerciseCategory { Id = id; Name = request.Name }

                return!
                    match result with
                    | Ok _ -> Successful.NO_CONTENT next ctx

                    | Error (RenameExerciseCategory.Validation error) ->
                        RequestErrors.BAD_REQUEST (ErrorResponse.from error) next ctx

                    | Error (RenameExerciseCategory.Domain (ExerciseCategoryNotFound as error)) ->
                        RequestErrors.NOT_FOUND (ErrorResponse.from error) next ctx

                    | Error (RenameExerciseCategory.Domain error) ->
                        RequestErrors.CONFLICT (ErrorResponse.from error) next ctx

                    | Error (RenameExerciseCategory.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }

    let delete (id: string) : HttpHandler = text $"delete %s{id}"
