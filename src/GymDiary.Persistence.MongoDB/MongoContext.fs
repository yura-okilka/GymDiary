namespace GymDiary.Persistence.MongoDB

open GymDiary.Persistence.MongoDB.Documents
open Microsoft.Extensions.Options
open MongoDB.Driver

type IMongoContext =
    abstract member Users: IMongoCollection<UserDocument>
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
    abstract member ExerciseDefinitions: IMongoCollection<ExerciseDefinitionDocument>
    abstract member Routines: IMongoCollection<RoutineDocument>
    abstract member Workouts: IMongoCollection<WorkoutDocument>

type MongoContext(mongoClient: IMongoClient, options: IOptions<MongoOptions>) =
    let databaseName = options.Value.Database

    member private _.GetCollection collection =
        mongoClient
            .GetDatabase(databaseName)
            .GetCollection<'TDocument>(collection)

    interface IMongoContext with
        member c.Users = c.GetCollection<UserDocument>("users")
        member c.ExerciseCategories = c.GetCollection<ExerciseCategoryDocument>("exerciseCategories")
        member c.ExerciseDefinitions = c.GetCollection<ExerciseDefinitionDocument>("exerciseDefinitions")
        member c.Routines = c.GetCollection<RoutineDocument>("routines")
        member c.Workouts = c.GetCollection<WorkoutDocument>("workouts")
