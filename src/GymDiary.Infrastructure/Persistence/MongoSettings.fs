namespace GymDiary.Infrastructure.Persistence

open Microsoft.Extensions.Configuration
open Validus

// F# types are not supported in settings retrieved from IConfiguration. Customize serializer or use FsConfig library if needed.

type MongoSettings = {
    Database: string
} with

    static member createFrom (section: string) (configuration: IConfiguration) : Result<MongoSettings, ValidationErrors> =
        let settingsOption = configuration.GetSection(section).Get<MongoSettings>()

        match settingsOption with
        | null -> ValidationErrors.create "settings" [ $"'{section}' settings must not be null" ] |> Error
        | settings -> validate {
            let! _ = Check.String.notEmpty (nameof settings.Database) settings.Database
            return settings
          }

    static member createFromOrThrow (section: string) (configuration: IConfiguration) : MongoSettings =
        match MongoSettings.createFrom section configuration with
        | Ok settings -> settings
        | Error errors ->
            errors
            |> ValidationErrors.toList
            |> String.concat "; "
            |> fun msg -> failwith $"Invalid '{section}' settings: {msg}"
