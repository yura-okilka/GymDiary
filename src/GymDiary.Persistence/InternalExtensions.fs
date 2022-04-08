namespace GymDiary.Persistence

open GymDiary.Core.Domain.Errors

open MongoDB.Driver

module internal InternalExtensions =

    module PersistenceError =

        let createResult (operation: string) (ex: exn) =
            match ex with
            | :? MongoException as ex -> Error(Database(operation, ex))
            | ex -> Error(Other(operation, ex))
