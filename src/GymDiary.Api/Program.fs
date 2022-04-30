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

module Program =

    let configureApp (root: CompositionRoot) (app: IApplicationBuilder) =
        app.UseGiraffe(Router.webApp root)

    let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore

    let configureSettings (builder: IConfigurationBuilder) (environment: string) =
        builder
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional = true)
            .AddJsonFile("appsettings.local.json", optional = true)
            .AddEnvironmentVariables("DOTNET_")
            .AddEnvironmentVariables("ASPNETCORE_")
            .AddEnvironmentVariables()

    [<EntryPoint>]
    let main args =
        PersistenceModule.configure () // MongoDB conventions must be configured before using MongoClient in the composition root.

        let env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        let configBuilder = (ConfigurationBuilder(), env) ||> configureSettings
        let settings = configBuilder.Build().Get<Settings>() // TODO: validate settings.
        let root = settings |> Trunk.compose |> CompositionRoot.compose

        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp root)
                    .ConfigureServices(configureServices)
                |> ignore)
            .Build()
            .Run()

        0 // Exit code
