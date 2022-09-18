namespace GymDiary.Api

open System
open System.Text.Json
open System.Text.Json.Serialization

open Giraffe

open GymDiary.Api
open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers
open GymDiary.Persistence

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open Validus

module Program =

    let configureSettings (builder: IConfigurationBuilder) (environment: string) =
        builder
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional = true)
            .AddJsonFile("appsettings.local.json", optional = true)
            .AddEnvironmentVariables("DOTNET_")
            .AddEnvironmentVariables("ASPNETCORE_")
            .AddEnvironmentVariables()
        |> ignore

    let configureAppConfiguration (context: WebHostBuilderContext) (builder: IConfigurationBuilder) (args: string []) =
        let env = context.HostingEnvironment.EnvironmentName
        builder.Sources.Clear()
        configureSettings builder env

        if args.Length > 0 then
            builder.AddCommandLine args |> ignore

    let configureLogging (builder: ILoggingBuilder) = builder.AddConsole().AddDebug() |> ignore

    let configureServices (services: IServiceCollection) =
        PersistenceModule.configure () // MongoDB conventions must be configured before using MongoClient in the composition root.

        services.AddGiraffe() |> ignore

        // Configure JSON serialization
        let jsonOptions = JsonSerializerOptions()
        jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase

        jsonOptions.Converters.Add(
            JsonFSharpConverter(
                unionTagName = "type",
                unionEncoding =
                    (JsonUnionEncoding.InternalTag
                     ||| JsonUnionEncoding.NamedFields
                     ||| JsonUnionEncoding.UnwrapOption
                     ||| JsonUnionEncoding.UnwrapSingleCaseUnions
                     ||| JsonUnionEncoding.AllowUnorderedTag)
            )
        )

        services.AddSingleton(jsonOptions) |> ignore
        services.AddSingleton<Json.ISerializer, SystemTextJson.Serializer>() |> ignore

    let configureApp (context: WebHostBuilderContext) (app: IApplicationBuilder) =
        let env = context.HostingEnvironment.EnvironmentName

        match env with
        | "Development" -> app.UseDeveloperExceptionPage() |> ignore
        | _ -> app.UseGiraffeErrorHandler(ErrorHandlers.unknownError) |> ignore

        let settings = context.Configuration.Get<AppSettings>()

        match AppSettings.validate settings with
        | Error errors ->
            errors
            |> ValidationErrors.toList
            |> String.concat "; "
            |> fun msg -> failwith $"Invalid settings: %s{msg}"
        | Ok _ -> ()

        let root = (settings, app.ApplicationServices) ||> Trunk.compose |> CompositionRoot.compose

        app.UseGiraffe(Router.webApp root)

    [<EntryPoint>]
    let main args =
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webHostBuilder ->
                webHostBuilder
                    .ConfigureAppConfiguration(fun context builder -> configureAppConfiguration context builder args)
                    .ConfigureLogging(configureLogging)
                    .ConfigureServices(configureServices)
                    .Configure(configureApp)
                |> ignore)
            .Build()
            .Run()

        0 // Exit code
