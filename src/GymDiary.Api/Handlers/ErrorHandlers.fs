module GymDiary.Api.Handlers.ErrorHandlers

open Oxpecker
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open type Microsoft.AspNetCore.Http.TypedResults

let resourceNotFound (ctx: HttpContext) =
    let logger = ctx.GetLogger()
    logger.LogWarning("Unhandled 404 error")
    ctx.Write <| NotFound {| Error = "Resource was not found" |}
