module GymDiary.Api.Oxpecker.Handlers.ExerciseDefinitionHandlers

open System
open Oxpecker
open Oxpecker.OpenApi
open System.Threading.Tasks
open GymDiary.Api.Oxpecker
open GymDiary.Core.Workflows.ExerciseDefinition
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

type CreateExerciseDefinitionRequest = {
    CategoryId: string
    OwnerId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
// TODO: add Sets: ExerciseSetsDTO
}

let createDefinition (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<CreateExerciseDefinition.ICommandHandler>()
        let! request = ctx.BindJson<CreateExerciseDefinitionRequest>()

        let! result =
            handler.Handle {
                CategoryId = request.CategoryId
                Name = request.Name
                Notes = request.Notes
                RestTime = request.RestTime
                //Sets = request.Sets
                OwnerId = "65e8edad477943d2b3844853"
            }
            |> Async.StartAsTask

        let response: IResult =
            match result with
            | Ok data -> Ok data // TODO: use Created
            | Error(CreateExerciseDefinition.InvalidCommand es) -> BadRequest(Responses.validationErrors es)
            | Error(CreateExerciseDefinition.CategoryNotFound e) -> Conflict(Responses.exerciseCategoryNotFound e)
            | Error(CreateExerciseDefinition.OwnerNotFound e) -> Conflict(Responses.ownerNotFound e)

        return! ctx.Write <| response
    }
    :> Task

let createDefinitionOpenApi =
    configureEndpoint _.WithTags("Exercise Definitions")
    >> addOpenApi (
        OpenApiConfig(
            requestBody = RequestBody(typeof<CreateExerciseDefinitionRequest>),
            responseBodies = [|
                ResponseBody(typeof<CreateExerciseDefinition.CommandResult>, ?statusCode = Some StatusCodes.Status200OK)
                ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status400BadRequest)
                ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status409Conflict)
            |],
            configureOperation =
                (fun o ->
                    o.OperationId <- "CreateExerciseDefinition"
                    o.Summary <- "Create exercise definition"
                    o)
        )
    )
