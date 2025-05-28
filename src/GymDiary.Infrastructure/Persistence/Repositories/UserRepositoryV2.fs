namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Application.Time
open GymDiary.Domain.Users
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type UserRepositoryV2(context: IMongoContext, mapper: IDocumentMapper<User, UserDocumentV2>, clock: IClock) =
    inherit EntityRepositoryBase<User, UserDocumentV2>(mapper, clock)
    override this.Collection = context.UsersV2

    interface IUserRepository with
        member this.ExistsWithEmail email =
            let email = email.Value
            this.Collection.Find(fun d -> d.Email = email).AnyAsync()
