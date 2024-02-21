namespace GymDiary.Persistence

open Microsoft.Extensions.Configuration
open Validus

// F# types are not supported in settings retrieved from IConfiguration. Customize serializer or use FsConfig library if needed.

type MongoSettings = {
    ConnectionString: string
    Database: string
} with

    static member createFrom (configuration: IConfiguration) (section: string) : Result<MongoSettings, ValidationErrors> = validate {
        let settings = configuration.GetSection(section).Get<MongoSettings>() // TODO: check null

        let! _ = Check.String.notEmpty (nameof settings.ConnectionString) settings.ConnectionString
        and! _ = Check.String.notEmpty (nameof settings.Database) settings.Database
        return settings
    }

    static member createFromOrThrow (configuration: IConfiguration) (section: string) : MongoSettings =
        match MongoSettings.createFrom configuration section with
        | Ok settings -> settings
        | Error errors ->
            errors
            |> ValidationErrors.toList
            |> String.concat "; "
            |> fun msg -> failwith $"Invalid '{section}' settings: {msg}"
