namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open MongoDB.Driver

module SportsmanRepository =

    let existWithId (collection: IMongoCollection<SportsmanDocument>) (sportsmanId: SportsmanId) : Async<bool> =
        let sportsmanId = sportsmanId |> Id.value

        MongoRepository.findAny collection (Expr.Quote(fun d -> d.Id = sportsmanId))
