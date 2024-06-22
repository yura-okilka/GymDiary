namespace GymDiary.Persistence.Repositories

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open FsToolkit.ErrorHandling

type UserRepository(repository: IMongoDocumentRepository<UserDocument>) =
    interface IUserRepository with

        member _.Create entity = async {
            let! createdDocument = entity |> UserDocument.fromDomain |> repository.InsertOne

            return
                createdDocument.Id
                |> Id.create<User> (nameof createdDocument.Id)
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<UserId>.Name, error)))
        }

        member _.ExistWithId userId =
            let userId = userId |> Id.value
            repository.Any(Expr.Quote(fun d -> d.Id = userId))

        member _.ExistWithEmail email =
            let email = email |> EmailAddress.value
            repository.Any(Expr.Quote(fun d -> d.Email = email))
