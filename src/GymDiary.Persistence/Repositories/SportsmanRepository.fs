namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling

open MongoDB.Driver

module SportsmanRepository =

    let private unwrapId id =
        let rawId = SportsmanId.value id
        (rawId, $"Sportsman with id '%s{rawId}'")

    let existWithId (collection: IMongoCollection<SportsmanDto>) (id: SportsmanId) =
        let id, entityWithIdMsg = unwrapId id

        task {
            try
                let! exists = collection.Find(fun d -> d.Id = id).AnyAsync()
                return Ok(exists)
            with
            | ObjectIdFormatException _ -> return Ok(false)
            | ex -> return PersistenceError.fromException $"find %s{entityWithIdMsg}" ex
        }
        |> Async.AwaitTask

    let createRepository (collection: IMongoCollection<SportsmanDto>) =
        { new ISportsmanRepository with
            member _.ExistWithId id = existWithId collection id }
