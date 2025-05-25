namespace GymDiary.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Documents
open MongoDB.Driver

type UserRepositoryV2(context: IMongoContext) =
    interface IUserRepository with

        member _.Create user = task {
            let document = user |> UserDocumentV2.fromDomain
            do! context.UsersV2.InsertOneAsync(document)
        }

        member _.ExistWithId id =
            let id = id.Value
            context.Users.Find(fun d -> d.Id = id).AnyAsync()

        member _.ExistWithEmail email =
            let email = email.Value
            context.Users.Find(fun d -> d.Email = email).AnyAsync()
