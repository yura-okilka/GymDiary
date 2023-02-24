namespace GymDiary.Api

open System.Runtime.CompilerServices
open System.Text.Json

open Giraffe

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

module JsonBinding =

    [<Extension>]
    type HttpContextExtensions() =

        [<Extension>]
        static member TryBindJsonAsync<'T>(ctx: HttpContext) = task {
            try
                let! json = ctx.BindJsonAsync<'T>()
                return Ok(json)
            with :? JsonException as ex ->
                return Error(ex)
        }

    let tryBindJson<'T>
        (parsingErrorHandler: JsonException -> ILogger -> HttpHandler)
        (successHandler: 'T -> HttpHandler)
        : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result = ctx.TryBindJsonAsync<'T>()
            let logger = ctx.GetLogger()

            return!
                match result with
                | Error ex -> (parsingErrorHandler ex logger) next ctx
                | Ok json -> (successHandler json) next ctx
        }
