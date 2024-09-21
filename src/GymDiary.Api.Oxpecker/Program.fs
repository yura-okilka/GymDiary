namespace GymDiary.Api.Oxpecker

#nowarn "20"

open Oxpecker
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open GymDiary.Core
open GymDiary.Persistence

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        let services = builder.Services
        services.AddRouting()
        services.AddOxpecker()
        services.AddCore()
        services.AddPersistence()

        let app = builder.Build()

        app.UseRouting()
        app.UseOxpecker(Router.webApp)

        app.Run()

        exitCode
