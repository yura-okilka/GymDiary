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
type PersistenceServiceExtensions() =
    [<Extension>]
    static member AddGymDiaryMongoDB(services: IServiceCollection) : IServiceCollection =
        SerializationSettings.register ()

        services
            .AddOptions<MongoOptions>()
            .BindConfiguration(MongoOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart()

        services
            .AddSingleton<IMongoClient, MongoClient>(fun sp ->
                new MongoClient(sp.GetRequiredService<IConfiguration>().GetConnectionStringOrThrow("gymdiary-db")))
            .AddSingleton<IMongoContext, MongoContext>()

            .AddSingleton<IDocumentMapper<User, UserDocument>, UserDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>, ExerciseCategoryDocumentMapper>()
            .AddSingleton<IDocumentMapper<ExerciseSetGroup, ExerciseSetDto list>, ExerciseSetDtoMapper>()
            .AddSingleton<IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>, ExerciseDefinitionDocumentMapper>()
            .AddSingleton<IDocumentMapper<Routine, RoutineDocument>, RoutineDocumentMapper>()

            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IExerciseCategoryRepository, ExerciseCategoryRepository>()
            .AddSingleton<IExerciseDefinitionRepository, ExerciseDefinitionRepository>()
            .AddSingleton<IRoutineRepository, RoutineRepository>()
