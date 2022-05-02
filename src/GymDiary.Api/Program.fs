namespace GymDiary.Api

open System

open Giraffe

open GymDiary.Api
open GymDiary.Api.DependencyInjection
open GymDiary.Persistence

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

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

    let configureLogging (builder: ILoggingBuilder) =
        builder
            .AddConsole()
            .AddDebug()
        |> ignore

    let configureServices (services: IServiceCollection) =
        PersistenceModule.configure () // MongoDB conventions must be configured before using MongoClient in the composition root.

        services.AddGiraffe() |> ignore

    let configureApp (context: WebHostBuilderContext) (app: IApplicationBuilder) =
        let env = context.HostingEnvironment.EnvironmentName

        match env with
        | "Development" -> app.UseDeveloperExceptionPage() |> ignore
        | _ -> app.UseGiraffeErrorHandler(ErrorHandler.handle) |> ignore

        let settings = context.Configuration.Get<Settings>() // TODO: validate settings.
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
