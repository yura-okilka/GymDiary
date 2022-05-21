namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions

open FsToolkit.ErrorHandling

open MongoDB.Driver

module SportsmanRepository =

    let private unwrapId id =
        let rawId = SportsmanId.value id
        (rawId, $"Sportsman with id '%s{rawId}'")

    let existWithId (collection: IMongoCollection<SportsmanDto>) (id: SportsmanId) =
        let id, entityWithIdMsg = unwrapId id

        MongoRepository.findAny collection (Expr.Quote(fun d -> d.Id = id))
        |> AsyncResult.mapError (PersistenceError.fromException $"find %s{entityWithIdMsg}")
