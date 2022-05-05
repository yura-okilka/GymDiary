namespace GymDiary.Api

open Validus

// F# types are not supported in settings retrieved from IConfiguration. Customize serializer or use FsConfig library if needed.

[<CLIMutable>]
type MongoDbSettings =
    { ConnectionString: string
      DatabaseName: string }

[<CLIMutable>]
type Settings = { MongoDb: MongoDbSettings }

module MongoDbSettings =

    let validate (settings: MongoDbSettings) : Result<MongoDbSettings, ValidationErrors> =
        validate {
            let notEmpty = Validators.Default.String.notEmpty

            let! _ = notEmpty (nameof settings.ConnectionString) settings.ConnectionString
            and! _ = notEmpty (nameof settings.DatabaseName) settings.DatabaseName

            return settings
        }

module Settings =

    let validate (settings: Settings) : Result<Settings, ValidationErrors> =
        validate {
            // Validate immediate primitive children. Delegate complex objects.
            let! _ = MongoDbSettings.validate settings.MongoDb

            return settings
        }
