namespace GymDiary.Persistence

#nowarn "20"

open System
open System.Runtime.CompilerServices
open GymDiary.Core.Persistence
open GymDiary.Persistence.Repositories
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver

[<Extension>]
type PersistenceServiceCollectionExtensions() =
    [<Extension>]
    static member AddPersistence(services: IServiceCollection) : IServiceCollection =
        SerializationSettings.register ()

        let newMongoRepository collection (sp: IServiceProvider) =
            MongoDocumentRepository(sp.GetRequiredService<IMongoClient>(), sp.GetRequiredService<MongoSettings>(), collection)

        // Create settings instance in the implementation factory to defer its creation and allow overriding IConfiguration in the test host.
        services.AddSingleton<MongoSettings>(fun sp -> MongoSettings.createFromOrThrow "MongoDb" (sp.GetRequiredService<IConfiguration>()))
        services.AddSingleton<IMongoClient, MongoClient>(fun sp -> MongoClient(sp.GetRequiredService<MongoSettings>().ConnectionString))

        services.AddSingleton<ISportsmanRepository, SportsmanRepository>(fun sp ->
            SportsmanRepository(newMongoRepository MongoCollections.Sportsmen sp))

        services.AddSingleton<IExerciseCategoryRepository, ExerciseCategoryRepository>(fun sp ->
            ExerciseCategoryRepository(newMongoRepository MongoCollections.ExerciseCategories sp))

        services.AddSingleton<IExerciseRepository, ExerciseRepository>(fun sp ->
            ExerciseRepository(newMongoRepository MongoCollections.Exercises sp))

        services
