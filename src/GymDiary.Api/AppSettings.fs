namespace GymDiary.Api

open Validus

// F# types are not supported in settings retrieved from IConfiguration. Customize serializer or use FsConfig library if needed.

[<CLIMutable>]
type MongoDbSettings = {
    ConnectionString: string
    DatabaseName: string
}

[<CLIMutable>]
type AppSettings = {
    MongoDb: MongoDbSettings
} with

    static member validate(settings: AppSettings) : Result<AppSettings, ValidationErrors> = validate {
        let mongoDb = settings.MongoDb

        let! _ = Check.String.notEmpty (nameof mongoDb.ConnectionString) mongoDb.ConnectionString
        and! _ = Check.String.notEmpty (nameof mongoDb.DatabaseName) mongoDb.DatabaseName

        return settings
    }
