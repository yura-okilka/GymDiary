module GymDiary.Api.Handlers.UserHandlers

open Oxpecker
open Oxpecker.OpenApi
open System.Threading.Tasks
open GymDiary.Api
open GymDiary.Core.Workflows.User
open Microsoft.AspNetCore.Http
open type Microsoft.AspNetCore.Http.TypedResults

let createUser (ctx: HttpContext) =
    task {
        let handler = ctx.GetService<CreateUser.ICommandHandler>()
        let! request = ctx.BindJson<CreateUser.Command>()

        let! result = handler.Handle request

        let response: IResult =
            match result with
            | Ok data -> Ok data // TODO: use Created
            | Error(CreateUser.InvalidCommand es) -> BadRequest(Responses.validationErrors es)
            | Error(CreateUser.UserAlreadyExists e) -> Conflict(Responses.userAlreadyExists e)

        return! ctx.Write <| response
    }
    :> Task

let createUserOpenApi =
    configureEndpoint _.WithTags("Users")
    >> addOpenApi (
        OpenApiConfig(
            requestBody = RequestBody(typeof<CreateUser.Command>),
            responseBodies = [|
                ResponseBody(typeof<CreateUser.CommandResult>, ?statusCode = Some StatusCodes.Status200OK)
                ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status400BadRequest)
                ResponseBody(typeof<ErrorResponse>, ?statusCode = Some StatusCodes.Status409Conflict)
            |],
            configureOperation =
                (fun o ->
                    o.OperationId <- "CreateUser"
                    o.Summary <- "Create user"
                    o)
        )
    )
