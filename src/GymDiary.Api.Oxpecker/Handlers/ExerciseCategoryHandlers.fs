module GymDiary.Api.Oxpecker.Handlers.ExerciseCategoryHandlers

open Oxpecker
open System.Threading.Tasks
open GymDiary.Api.Oxpecker
open GymDiary.Core.Workflows.ExerciseCategory
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

let getAllCategories (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<GetAllExerciseCategories.IQueryHandler>()

        let! result = handler.Handle { OwnerId = "65e8edad477943d2b3844853" } |> Async.StartAsTask

        let response: IResult =
            match result with
            | Ok data -> Ok data
            | Error(GetAllExerciseCategories.InvalidQuery e) -> BadRequest(Responses.validationError e)

        return! ctx.Write <| response
    }
    :> Task

let getCategoryById (id: string) (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<GetExerciseCategory.IQueryHandler>()

        let! result =
            handler.Handle {
                Id = id
                OwnerId = "65e8edad477943d2b3844853"
            }
            |> Async.StartAsTask

        let response: IResult =
            match result with
            | Ok data -> Ok data
            | Error(GetExerciseCategory.InvalidQuery es) -> BadRequest(Responses.validationErrors es)
            | Error(GetExerciseCategory.CategoryNotFound e) -> NotFound(Responses.exerciseCategoryNotFound e)

        return! ctx.Write <| response
    }
    :> Task

type CreateRequest = { Name: string }

let createCategory (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<CreateExerciseCategory.ICommandHandler>()
        let! request = ctx.BindJson<CreateRequest>()

        let! result =
            handler.Handle {
                Name = request.Name
                OwnerId = "65e8edad477943d2b3844853"
            }
            |> Async.StartAsTask

        let response: IResult =
            match result with
            | Ok data -> Ok data // TODO: use Created
            | Error(CreateExerciseCategory.InvalidCommand es) -> BadRequest(Responses.validationErrors es)
            | Error(CreateExerciseCategory.CategoryAlreadyExists e) -> Conflict(Responses.exerciseCategoryAlreadyExists e)
            | Error(CreateExerciseCategory.OwnerNotFound e) -> Conflict(Responses.ownerNotFound e)

        return! ctx.Write <| response
    }
    :> Task

type RenameRequest = { Name: string }

let renameCategory (id: string) (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<RenameExerciseCategory.ICommandHandler>()
        let! request = ctx.BindJson<RenameRequest>()

        let! result =
            handler.Handle {
                Id = id
                Name = request.Name
                OwnerId = "65e8edad477943d2b3844853"
            }
            |> Async.StartAsTask

        let response: IResult =
            match result with
            | Ok _ -> NoContent()
            | Error(RenameExerciseCategory.InvalidCommand es) -> BadRequest(Responses.validationErrors es)
            | Error(RenameExerciseCategory.CategoryNotFound e) -> NotFound(Responses.exerciseCategoryNotFound e)
            | Error(RenameExerciseCategory.NameAlreadyUsed e) -> Conflict(Responses.exerciseCategoryAlreadyExists e)

        return! ctx.Write <| response
    }
    :> Task
