namespace GymDiary.Persistence

open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type PersistenceSettings =
    { DatabaseName: string
      ConnectionString: string }

type IPersistenceCompositionRoot =
    abstract member ExerciseCategoryRepository: IExerciseCategoryRepository

module CompositionRoot =

    let compose (settings: PersistenceSettings) =
        let client = new MongoClient(settings.ConnectionString) // MongoClient & IMongoCollection<TDocument> are thread-safe.
        let context = DBContext.compose client settings.DatabaseName
        let exerciseCategoryRepository = ExerciseCategoryRepository.compose context.ExerciseCategories

        { new IPersistenceCompositionRoot with
            member _.ExerciseCategoryRepository = exerciseCategoryRepository }
