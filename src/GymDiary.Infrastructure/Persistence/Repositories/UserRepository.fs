namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type UserRepository(context: IMongoContext, mapper: IDocumentMapper<User, UserDocument>) =
    inherit EntityRepositoryBase<User, UserDocument>(context.Users, mapper)

    interface IUserRepository with
        member _.ExistsWithEmail email =
            let email = email.Value
            context.Users.Find(fun d -> d.Email = email).AnyAsync()
