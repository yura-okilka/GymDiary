namespace GymDiary.Api

#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open System.Text.Json
open System.Text.Json.Serialization

open Giraffe

open GymDiary.Api
open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers
open GymDiary.Persistence

open Validus

module Program =
    type TestEntryPoint() =
        class
        end

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        // MongoDB conventions must be configured before using MongoClient in the composition root.
        PersistenceModule.configure ()

        builder.Services.AddGiraffe()

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

        builder.Services.AddSingleton(jsonOptions)
        builder.Services.AddSingleton<Json.ISerializer, SystemTextJson.Serializer>()

        let app = builder.Build()

        match builder.Environment.EnvironmentName with
        | "Development" -> app.UseDeveloperExceptionPage()
        | _ -> app.UseGiraffeErrorHandler(ErrorHandlers.unknownError)

        let settings = builder.Configuration.Get<AppSettings>()

        match AppSettings.validate settings with
        | Error errors ->
            errors
            |> ValidationErrors.toList
            |> String.concat "; "
            |> fun msg -> failwith $"Invalid settings: %s{msg}"
        | Ok _ -> ()

        let root = (settings, app.Services) ||> Trunk.compose |> CompositionRoot.compose

        app.UseGiraffe(Router.webApp root)

        app.Run()

        exitCode
