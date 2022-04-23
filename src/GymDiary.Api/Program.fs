namespace GymDiary.Api

open Giraffe

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

module Program =

    let configureApp (app: IApplicationBuilder) = app.UseGiraffe(Router.webApp)

    let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main args =
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                |> ignore)
            .Build()
            .Run()

        0 // Exit code
