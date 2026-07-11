namespace GymDiary.Persistence.MongoDB.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open Microsoft.Extensions.Logging
open MongoDB.Driver

type UserRepository(context: IMongoContext, mapper: IDocumentMapper<User, UserDocument>, logger: ILogger<UserRepository>) =
    inherit EntityRepositoryBase<User, userId, UserDocument>(context.Users, mapper, logger)

    interface IUserRepository with
        member _.ExistsWithEmail email = async {
            let email = email.Value
            let! exists = context.Users.Find(fun d -> d.Email = email).AnyAsync() |> Async.AwaitTask
            // Don't log the raw email address (PII); the outcome is enough.
            logger.LogDebug("Checked user existence by email: {Exists}", exists)
            return exists
        }
