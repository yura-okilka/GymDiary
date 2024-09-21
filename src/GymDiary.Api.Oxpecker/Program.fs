namespace GymDiary.Api.Oxpecker

#nowarn "20"

open Oxpecker
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let webApp = [ route "/" <| text "Hello world"; route "/ping" <| text "pong" ]

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddRouting().AddOxpecker()

        let app = builder.Build()

        app.UseRouting()
        app.UseOxpecker(webApp)

        app.Run()

        exitCode
