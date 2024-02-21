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
    static member AddPersistence(services: IServiceCollection, configuration: IConfiguration) : IServiceCollection =
        SerializationSettings.register ()

        let mongoSettings = MongoSettings.createFromOrThrow configuration "MongoDb"

        let newMongoRepository collection (sp: IServiceProvider) =
            MongoRepository(sp.GetRequiredService<IMongoClient>(), sp.GetRequiredService<MongoSettings>(), collection)

        services.AddSingleton(mongoSettings)
        services.AddSingleton<IMongoClient>(MongoClient(mongoSettings.ConnectionString))

        services.AddSingleton<ISportsmanRepository, SportsmanRepository>(fun sp ->
            SportsmanRepository(newMongoRepository MongoCollections.Sportsmen sp))

        services.AddSingleton<IExerciseCategoryRepository, ExerciseCategoryRepository>(fun sp ->
            ExerciseCategoryRepository(newMongoRepository MongoCollections.ExerciseCategories sp))

        services.AddSingleton<IExerciseRepository, ExerciseRepository>(fun sp ->
            ExerciseRepository(newMongoRepository MongoCollections.Exercises sp))

        services
