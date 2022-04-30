namespace GymDiary.Api.DependencyInjection

open GymDiary.Api
open GymDiary.Api.DependencyInjection.Leaves

open MongoDB.Driver

/// Host of all the Leaves and common IO dependencies needed in different places.
type Trunk =
    { Persistence: Persistence }

module Trunk =

    let compose (settings: Settings) =
        let mongoClient = new MongoClient(settings.MongoDb.ConnectionString) // MongoClient & IMongoCollection<TDocument> are thread-safe.

        { Persistence = Persistence.compose mongoClient settings.MongoDb.DatabaseName }
