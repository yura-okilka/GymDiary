namespace GymDiary.Api

#nowarn "20"

open Oxpecker
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open GymDiary.Api.Handlers
open GymDiary.Core
open GymDiary.Persistence

module Program =
    type TestEntryPoint() =
        class
        end

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        let services = builder.Services
        services.AddRouting()
        services.AddOxpecker()
        services.AddEndpointsApiExplorer()
        services.AddSwaggerGen(_.CustomSchemaIds(_.FullName.Replace("+", "."))) // Error fix. https://github.com/swagger-api/swagger-ui/issues/7911
        services.AddGymDiaryCore()
        services.AddGymDiaryPersistence()

        let app = builder.Build()

        app.UseRouting()
        app.Use(ErrorHandlers.onError)
        app.UseOxpecker(Router.webApp)
        app.UseSwagger()
        app.UseSwaggerUI()
        app.Run(ErrorHandlers.resourceNotFound)

        app.Run()

        exitCode
