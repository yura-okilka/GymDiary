namespace GymDiary.Persistence.Repositories

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence

type SportsmanRepository(repository: IMongoRepository<SportsmanDocument>) =
    interface ISportsmanRepository with
        member _.ExistWithId sportsmanId =
            let sportsmanId = sportsmanId |> Id.value
            repository.Any(Expr.Quote(fun d -> d.Id = sportsmanId))
