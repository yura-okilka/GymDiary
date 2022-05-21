namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions

open FsToolkit.ErrorHandling

open MongoDB.Driver

module SportsmanRepository =

    let private sportsmanWithIdMsg id = $"Sportsman with id '%s{id}'"

    let existWithId (collection: IMongoCollection<SportsmanDto>) (SportsmanId sportsmanId) =
        let entityWithIdMsg = sportsmanWithIdMsg sportsmanId

        MongoRepository.findAny collection (Expr.Quote(fun d -> d.Id = sportsmanId))
        |> AsyncResult.mapError (PersistenceError.fromException $"find %s{entityWithIdMsg}")
