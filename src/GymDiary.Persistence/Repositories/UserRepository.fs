namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion
open MongoDB.Driver

type UserRepository(context: IMongoContext) =
    interface IUserRepository with

        member _.Create entity = task {
            let document = entity |> UserDocument.fromDomain
            do! context.Users.InsertOneAsync(document)
        }

        member _.ExistWithId id =
            let id = id |> Id.value
            context.Users.Find(fun d -> d.Id = id).AnyAsync()

        member _.ExistWithEmail email =
            let email = email |> EmailAddress.value
            context.Users.Find(fun d -> d.Email = email).AnyAsync()
