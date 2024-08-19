namespace GymDiary.Persistence

open GymDiary.Persistence
open MongoDB.Driver

type IMongoContext =
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
    abstract member Exercises: IMongoCollection<ExerciseDocument>
    abstract member Routines: IMongoCollection<RoutineDocument>
    abstract member WorkoutSessions: IMongoCollection<WorkoutSessionDocument>
    abstract member Users: IMongoCollection<UserDocument>

type MongoContext(mongoClient: IMongoClient, mongoSettings: MongoSettings) =
    member private _.GetCollection collection =
        mongoClient
            .GetDatabase(mongoSettings.Database)
            .GetCollection<'TDocument>(collection)

    interface IMongoContext with
        member c.ExerciseCategories = c.GetCollection<ExerciseCategoryDocument>("exerciseCategories")
        member c.Exercises = c.GetCollection<ExerciseDocument>("exercises")
        member c.Routines = c.GetCollection<RoutineDocument>("routines")
        member c.WorkoutSessions = c.GetCollection<WorkoutSessionDocument>("workoutSessions")
        member c.Users = c.GetCollection<UserDocument>("users")
