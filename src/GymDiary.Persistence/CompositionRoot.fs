namespace GymDiary.Persistence

open GymDiary.Persistence
open GymDiary.Persistence.Services

open MongoDB.Driver

type PersistenceSettings =
    { DatabaseName: string
      ConnectionString: string }

type IPersistenceCompositionRoot =
    abstract member ExerciseCategoryService: IExerciseCategoryService

module CompositionRoot =

    let compose (settings: PersistenceSettings) =
        let client = new MongoClient(settings.ConnectionString) // MongoClient & IMongoCollection<TDocument> are thread-safe.
        let context = DBContext.compose client settings.DatabaseName
        let exerciseCategoryService = ExerciseCategoryService.compose context.ExerciseCategories

        { new IPersistenceCompositionRoot with
            member _.ExerciseCategoryService = exerciseCategoryService }
