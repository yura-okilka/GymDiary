namespace GymDiary.Persistence.Repositories

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling

type SportsmanRepository(repository: IMongoRepository<SportsmanDocument>) =
    interface ISportsmanRepository with

        member _.Create entity = async {
            let! createdDocument = entity |> SportsmanDocument.fromDomain |> repository.InsertOne

            return
                createdDocument.Id
                |> Id.create<Sportsman> (nameof createdDocument.Id)
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<SportsmanId>.Name, error)))
        }

        member _.ExistWithId sportsmanId =
            let sportsmanId = sportsmanId |> Id.value
            repository.Any(Expr.Quote(fun d -> d.Id = sportsmanId))

        member _.ExistWithEmail email =
            let email = email |> EmailAddress.value
            repository.Any(Expr.Quote(fun d -> d.Email = email))
