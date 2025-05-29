namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type UserRepositoryV2(context: IMongoContext, mapper: IDocumentMapper<User, UserDocumentV2>) =
    inherit EntityRepositoryBase<User, UserDocumentV2>(context.UsersV2, mapper)

    interface IUserRepository with
        member _.ExistsWithEmail email =
            let email = email.Value
            context.UsersV2.Find(fun d -> d.Email = email).AnyAsync()
