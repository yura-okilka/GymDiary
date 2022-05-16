namespace GymDiary.Persistence

open System

open GymDiary.Core.Domain

open MongoDB.Driver

module internal InternalExtensions =

    [<AutoOpen>]
    module Interop =

        let inline isNull value = obj.ReferenceEquals(value, null)

        let inline defaultof<'T> = Unchecked.defaultof<'T>

    [<AutoOpen>]
    module ExceptionPatterns =

        let (|ObjectIdFormatException|_|) (ex: exn) =
            match ex with
            | :? FormatException as fex when fex.Message.Contains("is not a valid 24 digit hex string") -> Some fex
            | _ -> None

    module PersistenceError =

        let fromException (operation: string) (ex: exn) =
            match ex with
            | :? MongoException -> Error(Database(operation, ex))
            | _ -> Error(Other(operation, ex))
