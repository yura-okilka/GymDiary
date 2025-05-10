namespace GymDiary.Api

#nowarn "20"

open System.Collections.Generic
open Oxpecker
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http.Features
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

        services.AddProblemDetails(fun options ->
            options.CustomizeProblemDetails <-
                fun context ->
                    let activity = context.HttpContext.Features.Get<IHttpActivityFeature>().Activity
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id) |> ignore
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier) |> ignore)

        services.AddGymDiaryCore()
        services.AddGymDiaryPersistence()

        let app = builder.Build()

        app.UseRouting()
        app.UseExceptionHandler()
        app.UseOxpecker(Router.webApp)
        app.UseSwagger()
        app.UseSwaggerUI()
        app.Run(ErrorHandlers.resourceNotFound)

        app.Run()

        exitCode
