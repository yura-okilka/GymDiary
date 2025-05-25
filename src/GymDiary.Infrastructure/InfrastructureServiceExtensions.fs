namespace GymDiary.Infrastructure.Persistence

#nowarn "20"

open System
open System.Runtime.CompilerServices
open GymDiary.Application.Time
open GymDiary.Core.Persistence
open GymDiary.Infrastructure.Time
open GymDiary.Infrastructure.Persistence.Repositories
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver

[<Extension>]
type InfrastructureServiceExtensions() =
    [<Extension>]
    static member AddGymDiaryInfrastructure(services: IServiceCollection) : IServiceCollection =
        SerializationSettings.register ()

        // Create settings instance in the implementation factory to defer its creation and allow overriding IConfiguration in the test host.
        services
            .AddSingleton<IClock>(SystemClock(TimeProvider.System))
            .AddSingleton<MongoSettings>(fun sp -> MongoSettings.createFromOrThrow "MongoDb" (sp.GetRequiredService<IConfiguration>()))
            .AddSingleton<IMongoClient, MongoClient>(fun sp ->
                new MongoClient(
                    sp
                        .GetRequiredService<IConfiguration>()
                        .GetConnectionString("gymdiary-db") // TODO: GetConnectionStringOrThrow
                ))
            .AddSingleton<IMongoContext, MongoContext>()
            .AddSingleton<GymDiary.Application.Persistence.IIdProvider, MongoObjectIdProvider>()
            .AddSingleton<GymDiary.Application.Persistence.IUserRepository, UserRepositoryV2>()
            .AddSingleton<GymDiary.Application.Persistence.IExerciseCategoryRepository, ExerciseCategoryRepositoryV2>()
            .AddSingleton<IExerciseDefinitionRepository, ExerciseDefinitionRepository>()
