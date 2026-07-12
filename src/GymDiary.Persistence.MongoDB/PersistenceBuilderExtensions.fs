namespace GymDiary.Persistence.MongoDB

#nowarn "20"

open System.Runtime.CompilerServices
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open GymDiary.Persistence.MongoDB.Repositories
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

[<Extension>]
type PersistenceBuilderExtensions() =
    [<Extension>]
    static member AddGymDiaryPersistenceMongoDB(builder: IHostApplicationBuilder) : IHostApplicationBuilder =
        SerializationSettings.register ()

        // Aspire MongoDB client integration: registers IMongoClient/IMongoDatabase from the AppHost
        // connection string, plus health checks and telemetry.
        builder.AddMongoDBClient("gymdiary-db")

        builder.Services
            .AddSingleton<IMongoContext, MongoContext>()

            .AddSingleton<IDocumentMapper<User, UserDocument>, UserDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>, ExerciseCategoryDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto>, ExerciseSetGroupMapper>()
            .AddSingleton<IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>, ExerciseDefinitionDocumentMapper>()
            .AddSingleton<IDocumentMapper<Routine, RoutineDocument>, RoutineDocumentMapper>()

            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IExerciseCategoryRepository, ExerciseCategoryRepository>()
            .AddSingleton<IExerciseDefinitionRepository, ExerciseDefinitionRepository>()
            .AddSingleton<IRoutineRepository, RoutineRepository>()

        builder
