namespace GymDiary.Persistence

open System

open GymDiary.Core.Domain

open MongoDB.Driver

module internal InternalExtensions =

    [<AutoOpen>]
    module ExceptionPatterns =

        let (|ObjectIdFormatException|_|) (ex: exn) =
            match ex with
            | :? FormatException as fex when fex.Message.Contains("is not a valid 24 digit hex string") -> Some fex
            | _ -> None

    module PersistenceError =

        let fromException (operation: string) (ex: exn) =
            match ex with
            | :? MongoException -> DatabaseError(operation, ex)
            | _ -> OtherError(operation, ex)
