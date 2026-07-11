namespace GymDiary.Persistence.MongoDB.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open MongoDB.Driver

type UserRepository(context: IMongoContext, mapper: IDocumentMapper<User, UserDocument>) =
    inherit EntityRepositoryBase<User, userId, UserDocument>(context.Users, mapper)
    interface IUserRepository with
        member _.ExistsWithEmail email =
            let email = email.Value
            context.Users.Find(fun d -> d.Email = email).AnyAsync() |> Async.AwaitTask
