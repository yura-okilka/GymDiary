namespace GymDiary.Api

#nowarn "20"

open System.Collections.Generic
open Oxpecker
open Oxpecker.OpenApi
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http.Features
open Microsoft.Extensions.DependencyInjection
open Scalar.AspNetCore
open GymDiary.Core
open GymDiary.Infrastructure.Persistence

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

        services.AddOpenApi(fun o -> o.AddSchemaTransformer<FSharpOptionSchemaTransformer>() |> ignore)

        services.AddProblemDetails(fun options ->
            options.CustomizeProblemDetails <-
                fun context ->
                    let activity =
                        context.HttpContext.Features
                            .GetRequiredFeature<IHttpActivityFeature>()
                            .Activity

                    context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id) |> ignore
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier) |> ignore)

        services.AddExceptionHandler<OxpeckerExceptionHandler>()

        services.AddGymDiaryCore()
        services.AddGymDiaryInfrastructure()

        let app = builder.Build()

        app.UseRouting()
        app.UseExceptionHandler()
        app.UseOxpecker(Router.webApp)
        app.MapOpenApi()
        app.MapScalarApiReference()

        app.Run()

        exitCode
