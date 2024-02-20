namespace GymDiary.Api

#nowarn "20"

open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open GymDiary.Api
open GymDiary.Api.DependencyInjection
open GymDiary.Api.HttpHandlers
open GymDiary.Persistence

module Program =
    type TestEntryPoint() =
        class
        end

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

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
        builder.Services.AddPersistence(builder.Configuration)

        let app = builder.Build()

        match builder.Environment.EnvironmentName with
        | "Development" -> app.UseDeveloperExceptionPage()
        | _ -> app.UseGiraffeErrorHandler(ErrorHandlers.unknownError)

        let root = app.Services |> CompositionRoot.compose

        app.UseGiraffe(Router.webApp root)

        app.Run()

        exitCode
