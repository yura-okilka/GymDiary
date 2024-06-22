namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Workflows.User

open Microsoft.AspNetCore.Http

module UserHandlers =

    let create (createUser: CreateUser.Workflow) (request: CreateUser.Command) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result = createUser request

            let handler =
                match result with
                | Ok data -> Successful.CREATED data
                | Error(CreateUser.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(CreateUser.UserAlreadyExists e) -> RequestErrors.CONFLICT(Responses.userAlreadyExists e)

            return! handler next ctx
        }
