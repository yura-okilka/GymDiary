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
        services.AddEndpointsApiExplorer()
        services.AddSwaggerGen(fun o -> o.CustomSchemaIds(fun t -> t.FullName.Replace("+", "."))) // Error fix. https://github.com/swagger-api/swagger-ui/issues/7911
        services.AddCore()
        services.AddPersistence()

        let app = builder.Build()

        app.UseRouting()
        app.UseOxpecker(Router.webApp)
        app.UseSwagger()
        app.UseSwaggerUI()

        app.Run()

        exitCode
