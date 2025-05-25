namespace GymDiary.Persistence

open System

module internal InternalExtensions =

    [<AutoOpen>]
    module ExceptionPatterns =

        let (|ObjectIdFormatException|_|) (ex: exn) =
            match ex with
            | :? FormatException as fex when fex.Message.Contains("is not a valid 24 digit hex string") -> Some fex
            | _ -> None
