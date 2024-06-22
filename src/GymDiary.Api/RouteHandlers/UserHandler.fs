namespace GymDiary.Api.RouteHandlers

open System.Threading.Tasks
open GymDiary.Api
open GymDiary.Core.Workflows.User
open Microsoft.AspNetCore.Http

type UserHandler() =
    static member Create(createUser: CreateUser.Workflow, request: CreateUser.Command) : Task<IResult> = task {
        let! result = createUser request

        return
            match result with
            | Ok data -> Results.Ok(data) // TODO: use Results.Created
            | Error(CreateUser.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
            | Error(CreateUser.UserAlreadyExists e) -> Results.Conflict(Responses.userAlreadyExists e)
    }
