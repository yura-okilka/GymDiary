namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Domain
open GymDiary.Core.Workflows.ExerciseCategory

open Microsoft.AspNetCore.Http

module ExerciseCategoryHandlers =

    type CreateRequest = { Name: string }

    let create (createExerciseCategory: CreateExerciseCategory.Workflow) (sportsmanId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<CreateRequest>()

                let! result =
                    createExerciseCategory
                        { Name = request.Name
                          OwnerId = sportsmanId }

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

    let getAll (getAllExerciseCategories: GetAllExerciseCategories.Workflow) (sportsmanId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = getAllExerciseCategories { OwnerId = sportsmanId }

                return!
                    match result with
                    | Ok data -> Successful.OK data next ctx

                    | Error (GetAllExerciseCategories.Validation errors) ->
                        RequestErrors.BAD_REQUEST (ErrorResponse.from errors) next ctx

                    | Error (GetAllExerciseCategories.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }

    let getById
        (getExerciseCategory: GetExerciseCategory.Workflow)
        (sportsmanId: string)
        (categoryId: string)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result =
                    getExerciseCategory
                        { Id = categoryId
                          OwnerId = sportsmanId }

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

    let rename
        (renameExerciseCategory: RenameExerciseCategory.Workflow)
        (sportsmanId: string)
        (categoryId: string)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<RenameRequest>()

                let! result =
                    renameExerciseCategory
                        { Id = categoryId
                          OwnerId = sportsmanId
                          Name = request.Name }

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

    let delete
        (deleteExerciseCategory: DeleteExerciseCategory.Workflow)
        (sportsmanId: string)
        (categoryId: string)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = deleteExerciseCategory { Id = categoryId } // TODO: use sportsmanId.

                return!
                    match result with
                    | Ok _ -> Successful.NO_CONTENT next ctx

                    | Error (DeleteExerciseCategory.Domain (ExerciseCategoryNotFound as error)) ->
                        RequestErrors.NOT_FOUND (ErrorResponse.from error) next ctx

                    | Error (DeleteExerciseCategory.Domain error) ->
                        RequestErrors.CONFLICT (ErrorResponse.from error) next ctx

                    | Error (DeleteExerciseCategory.Persistence error) ->
                        ServerErrors.INTERNAL_ERROR (ErrorResponse.from error) next ctx
            }
