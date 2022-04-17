namespace GymDiary.Persistence

open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type PersistenceSettings =
    { DatabaseName: string
      ConnectionString: string }

module PersistenceRoot =

    let createRoot (settings: PersistenceSettings) =
        let client = new MongoClient(settings.ConnectionString) // MongoClient & IMongoCollection<TDocument> are thread-safe.
        let context = DBContext.createContext client settings.DatabaseName
        let exerciseCategoryRepository = ExerciseCategoryRepository.createRepository context.ExerciseCategories
        let exerciseTemplateRepository = ExerciseTemplateRepository.createRepository context.ExerciseTemplates
        let sportsmanRepository = SportsmanRepository.createRepository context.Sportsmans

        { new IPersistenceCompositionRoot with
            member _.ExerciseCategoryRepository = exerciseCategoryRepository
            member _.ExerciseTemplateRepository = exerciseTemplateRepository
            member _.SportsmanRepository = sportsmanRepository }
