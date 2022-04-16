namespace GymDiary.Persistence

open GymDiary.Core.Domain.Errors

open MongoDB.Driver

module internal InternalExtensions =

    [<AutoOpen>]
    module Interop =

        let inline isNull value = obj.ReferenceEquals(value, null)

        let inline aNull<'T> = Unchecked.defaultof<'T>

    module PersistenceError =

        let fromException (operation: string) (ex: exn) =
            match ex with
            | :? MongoException -> Error(Database(operation, ex))
            | _ -> Error(Other(operation, ex))
