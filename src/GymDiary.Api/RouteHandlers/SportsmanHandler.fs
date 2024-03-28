namespace GymDiary.Api.RouteHandlers

open System.Threading.Tasks
open GymDiary.Api
open GymDiary.Core.Workflows.Sportsman
open Microsoft.AspNetCore.Http

type SportsmanHandler() =
    static member Create(createSportsman: CreateSportsman.Workflow, request: CreateSportsman.Command) : Task<IResult> = task {
        let! result = createSportsman request

        return
            match result with
            | Ok data -> Results.Ok(data) // TODO: use Results.Created
            | Error(CreateSportsman.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
            | Error(CreateSportsman.SportsmanAlreadyExists e) -> Results.Conflict(Responses.sportsmanAlreadyExists e)
    }
