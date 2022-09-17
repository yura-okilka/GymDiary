namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Workflows.ExerciseCategory

open Microsoft.AspNetCore.Http

module ExerciseCategoryHandlers =

    type CreateRequest = { Name: string }

    let create
        (createExerciseCategory: CreateExerciseCategory.Workflow)
        (sportsmanId: string)
        (request: CreateRequest)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result =
                    createExerciseCategory
                        {
                            Name = request.Name
                            OwnerId = sportsmanId
                        }

                return!
                    match result with
                    | Ok data -> Successful.CREATED data next ctx
                    | Error error ->
                        let message = CreateExerciseCategory.CommandError.toString error

                        match error with
                        | CreateExerciseCategory.InvalidCommand errors ->
                            RequestErrors.BAD_REQUEST (ErrorResponse.validationErrors errors) next ctx

                        | CreateExerciseCategory.CategoryAlreadyExists _ ->
                            RequestErrors.CONFLICT (ErrorResponse.exerciseCategoryAlreadyExists message) next ctx

                        | CreateExerciseCategory.OwnerNotFound _ ->
                            RequestErrors.CONFLICT (ErrorResponse.ownerNotFound message) next ctx
            }

    let getAll (getAllExerciseCategories: GetAllExerciseCategories.Workflow) (sportsmanId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result = getAllExerciseCategories { OwnerId = sportsmanId }

                return!
                    match result with
                    | Ok data -> Successful.OK data next ctx

                    | Error (GetAllExerciseCategories.InvalidQuery error) ->
                        RequestErrors.BAD_REQUEST (ErrorResponse.validationError error) next ctx
            }

    let getById (getExerciseCategory: GetExerciseCategory.Workflow) (sportsmanId: string, categoryId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result =
                    getExerciseCategory
                        {
                            Id = categoryId
                            OwnerId = sportsmanId
                        }

                return!
                    match result with
                    | Ok data -> Successful.OK data next ctx
                    | Error error ->
                        let message = GetExerciseCategory.QueryError.toString error

                        match error with
                        | GetExerciseCategory.InvalidQuery errors ->
                            RequestErrors.BAD_REQUEST (ErrorResponse.validationErrors errors) next ctx

                        | GetExerciseCategory.CategoryNotFound _ ->
                            RequestErrors.NOT_FOUND (ErrorResponse.exerciseCategoryNotFound message) next ctx
            }

    type RenameRequest = { Name: string }

    let rename
        (renameExerciseCategory: RenameExerciseCategory.Workflow)
        (sportsmanId: string, categoryId: string)
        (request: RenameRequest)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result =
                    renameExerciseCategory
                        {
                            Id = categoryId
                            OwnerId = sportsmanId
                            Name = request.Name
                        }

                return!
                    match result with
                    | Ok _ -> Successful.NO_CONTENT next ctx
                    | Error error ->
                        let message = RenameExerciseCategory.CommandError.toString error

                        match error with
                        | RenameExerciseCategory.InvalidCommand errors ->
                            RequestErrors.BAD_REQUEST (ErrorResponse.validationErrors errors) next ctx

                        | RenameExerciseCategory.CategoryNotFound _ ->
                            RequestErrors.NOT_FOUND (ErrorResponse.exerciseCategoryNotFound message) next ctx

                        | RenameExerciseCategory.NameAlreadyUsed _ ->
                            RequestErrors.CONFLICT (ErrorResponse.nameAlreadyUsed message) next ctx
            }

    let delete (deleteExerciseCategory: DeleteExerciseCategory.Workflow) (sportsmanId: string, categoryId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! result =
                    deleteExerciseCategory
                        {
                            Id = categoryId
                            OwnerId = sportsmanId
                        }

                return!
                    match result with
                    | Ok _ -> Successful.NO_CONTENT next ctx
                    | Error error ->
                        let message = DeleteExerciseCategory.CommandError.toString error

                        match error with
                        | DeleteExerciseCategory.InvalidCommand errors ->
                            RequestErrors.BAD_REQUEST (ErrorResponse.validationErrors errors) next ctx

                        | DeleteExerciseCategory.CategoryNotFound _ ->
                            RequestErrors.NOT_FOUND (ErrorResponse.exerciseCategoryNotFound message) next ctx
            }
