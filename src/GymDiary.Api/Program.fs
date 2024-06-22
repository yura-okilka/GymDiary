namespace GymDiary.Api

#nowarn "20"

open System
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open GymDiary.Core.Time
open GymDiary.Api.Endpoints
open GymDiary.Api.DependencyInjection
open GymDiary.Persistence

module Program =
    type TestEntryPoint() =
        class
        end

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()
        // TODO: is it needed?
        // Error fix. https://github.com/swagger-api/swagger-ui/issues/7911
        builder.Services.AddSwaggerGen(fun o -> o.CustomSchemaIds(fun t -> t.FullName.Replace("+", ".")))

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

        jsonOptions.Converters.Add(JsonStringEnumConverter())

        builder.Services.AddSingleton(jsonOptions)
        builder.Services.AddSingleton<Json.ISerializer, SystemTextJson.Serializer>()
        builder.Services.AddSingleton<IClock>(SystemClock(TimeProvider.System))
        builder.Services.AddPersistence()

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        let root = app.Services |> CompositionRoot.compose

        app.MapGymDiaryApi(root)

        app.Run()

        exitCode
