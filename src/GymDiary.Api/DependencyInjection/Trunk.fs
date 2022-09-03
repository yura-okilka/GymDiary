namespace GymDiary.Api.DependencyInjection

open System

open GymDiary.Api
open GymDiary.Api.DependencyInjection.Leaves

open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open MongoDB.Driver

/// Host of all the Leaves and common IO dependencies needed in different places.
type Trunk =
    {
        Logger: ILogger
        Persistence: Persistence
    }

module Trunk =

    /// Composes Trunk with IO dependencies. Only dependencies from framework and libraries should be taken from service provider.
    /// It is the way to take the best of composition root and ASP.NET Core features.
    let compose (settings: Settings) (serviceProvider: IServiceProvider) =
        let mongoClient = new MongoClient(settings.MongoDb.ConnectionString) // MongoClient & IMongoCollection<TDocument> are thread-safe.

        {
            Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger()
            Persistence = Persistence.compose mongoClient settings.MongoDb.DatabaseName
        }
