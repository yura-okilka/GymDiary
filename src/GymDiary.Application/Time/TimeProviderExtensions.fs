namespace GymDiary.Application.Time

open System

[<AutoOpen>]
module TimeProviderExtensions =

    type TimeProvider with

        /// Current UTC time as a DateTime — shorthand for GetUtcNow().UtcDateTime.
        member this.UtcNow = this.GetUtcNow().UtcDateTime
