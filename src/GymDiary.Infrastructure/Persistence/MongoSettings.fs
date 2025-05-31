namespace GymDiary.Infrastructure.Persistence

open Microsoft.Extensions.Configuration
open Validus

// F# types are not supported in settings retrieved from IConfiguration. Customize serializer or use FsConfig library if needed.

type MongoSettings = {
    Database: string
} with

    static member Section = "MongoDb"

    static member createFrom(configuration: IConfiguration) : Result<MongoSettings, ValidationErrors> =
        match configuration.GetSection(MongoSettings.Section).Get<MongoSettings>() with
        | null -> ValidationErrors.create "settings" [ $"{MongoSettings.Section} settings must not be null" ] |> Error
        | settings -> validate {
            let! _ = Check.String.notEmpty (nameof settings.Database) settings.Database
            return settings
          }

    static member createFromOrThrow(configuration: IConfiguration) : MongoSettings =
        match MongoSettings.createFrom configuration with
        | Ok settings -> settings
        | Error errors ->
            errors
            |> ValidationErrors.toList
            |> String.concat "; "
            |> fun msg -> failwith $"Invalid {MongoSettings.Section} settings: {msg}"
