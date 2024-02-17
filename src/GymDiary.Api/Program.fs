namespace GymDiary.Api

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

    [<EntryPoint>]
    let main args =
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webHostBuilder ->
                webHostBuilder
                    .ConfigureLogging(fun b -> b.AddConsole().AddDebug() |> ignore)
                    .ConfigureServices(fun services ->
                        // MongoDB conventions must be configured before using MongoClient in the composition root.
                        PersistenceModule.configure ()

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
                        services.AddSingleton<Json.ISerializer, SystemTextJson.Serializer>() |> ignore)

                    .Configure(fun context app ->
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

                        app.UseGiraffe(Router.webApp root))
                |> ignore)
            .Build()
            .Run()

        0 // Exit code
