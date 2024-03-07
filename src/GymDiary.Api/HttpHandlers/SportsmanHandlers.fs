namespace GymDiary.Api.HttpHandlers

open Giraffe

open GymDiary.Api
open GymDiary.Core.Workflows.Sportsman

open Microsoft.AspNetCore.Http

module SportsmanHandlers =

    let create (createSportsman: CreateSportsman.Workflow) (request: CreateSportsman.Command) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result = createSportsman request

            let handler =
                match result with
                | Ok data -> Successful.CREATED data
                | Error(CreateSportsman.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(CreateSportsman.SportsmanAlreadyExists e) -> RequestErrors.CONFLICT(Responses.sportsmanAlreadyExists e)

            return! handler next ctx
        }
