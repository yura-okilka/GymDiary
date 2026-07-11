namespace GymDiary.Persistence.MongoDB

open System.ComponentModel.DataAnnotations

// [<CLIMutable>] gives the F# record a parameterless constructor and settable properties
// so the options/configuration binder can populate it from IConfiguration.
[<CLIMutable>]
type MongoOptions = {
    [<Required>]
    Database: string
} with

    static member Section = "MongoDb"
