namespace GymDiary.Persistence.MongoDB

#nowarn "20"

open System.Runtime.CompilerServices
open Common.Configuration
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open GymDiary.Persistence.MongoDB.Repositories
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver

[<Extension>]
type MongoDBServiceExtensions() =
    [<Extension>]
    static member AddGymDiaryMongoDB(services: IServiceCollection) : IServiceCollection =
        SerializationSettings.register ()

        // Create settings instance in the implementation factory to defer its creation and allow overriding IConfiguration in the test host.
        services
            .AddSingleton<MongoSettings>(fun sp -> MongoSettings.createFromOrThrow (sp.GetRequiredService<IConfiguration>()))
            .AddSingleton<IMongoClient, MongoClient>(fun sp ->
                new MongoClient(sp.GetRequiredService<IConfiguration>().GetConnectionStringOrThrow("gymdiary-db")))
            .AddSingleton<IMongoContext, MongoContext>()
            .AddSingleton<IEntityIdFactory, MongoObjectIdFactory>()

            .AddSingleton<IDocumentMapper<User, UserDocument>, UserDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>, ExerciseCategoryDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseSets, ExerciseSetDto list>, ExerciseSetDtoMapper>()
            .AddSingleton<IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>, ExerciseDefinitionDocumentMapper>()
            .AddSingleton<IDocumentMapper<Routine, RoutineDocument>, RoutineDocumentMapper>()

            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IExerciseCategoryRepository, ExerciseCategoryRepository>()
            .AddSingleton<IExerciseDefinitionRepository, ExerciseDefinitionRepository>()
            .AddSingleton<IRoutineRepository, RoutineRepository>()

