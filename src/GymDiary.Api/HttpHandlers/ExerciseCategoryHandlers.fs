namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Workflows.ExerciseCategory

open Microsoft.AspNetCore.Http

module ExerciseCategoryHandlers =

    type CreateRequest = { Name: string }

    let create (createExerciseCategory: CreateExerciseCategory.Workflow) (sportsmanId: string) (request: CreateRequest) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                createExerciseCategory {
                    Name = request.Name
                    OwnerId = sportsmanId
                }

            let handler =
                match result with
                | Ok data -> Successful.CREATED data
                | Error(CreateExerciseCategory.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(CreateExerciseCategory.CategoryAlreadyExists e) -> RequestErrors.CONFLICT(Responses.exerciseCategoryAlreadyExists e)
                | Error(CreateExerciseCategory.OwnerNotFound e) -> RequestErrors.CONFLICT(Responses.ownerNotFound e)

            return! handler next ctx
        }

    let getAll (getAllExerciseCategories: GetAllExerciseCategories.Workflow) (sportsmanId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result = getAllExerciseCategories { OwnerId = sportsmanId }

            let handler =
                match result with
                | Ok data -> Successful.OK data
                | Error(GetAllExerciseCategories.InvalidQuery e) -> RequestErrors.BAD_REQUEST(Responses.validationError e)

            return! handler next ctx
        }

    let getById (getExerciseCategory: GetExerciseCategory.Workflow) (sportsmanId: string, categoryId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                getExerciseCategory {
                    Id = categoryId
                    OwnerId = sportsmanId
                }

            let handler =
                match result with
                | Ok data -> Successful.OK data
                | Error(GetExerciseCategory.InvalidQuery es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(GetExerciseCategory.CategoryNotFound e) -> RequestErrors.NOT_FOUND(Responses.exerciseCategoryNotFound e)

            return! handler next ctx
        }

    type RenameRequest = { Name: string }

    let rename
        (renameExerciseCategory: RenameExerciseCategory.Workflow)
        (sportsmanId: string, categoryId: string)
        (request: RenameRequest)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                renameExerciseCategory {
                    Id = categoryId
                    OwnerId = sportsmanId
                    Name = request.Name
                }

            let handler =
                match result with
                | Ok _ -> Successful.NO_CONTENT
                | Error(RenameExerciseCategory.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(RenameExerciseCategory.CategoryNotFound e) -> RequestErrors.NOT_FOUND(Responses.exerciseCategoryNotFound e)
                | Error(RenameExerciseCategory.NameAlreadyUsed e) -> RequestErrors.CONFLICT(Responses.exerciseCategoryAlreadyExists e)

            return! handler next ctx
        }

    let delete (deleteExerciseCategory: DeleteExerciseCategory.Workflow) (sportsmanId: string, categoryId: string) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                deleteExerciseCategory {
                    Id = categoryId
                    OwnerId = sportsmanId
                }

            let handler =
                match result with
                | Ok _ -> Successful.NO_CONTENT
                | Error(DeleteExerciseCategory.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(DeleteExerciseCategory.CategoryNotFound e) -> RequestErrors.NOT_FOUND(Responses.exerciseCategoryNotFound e)

            return! handler next ctx
        }
