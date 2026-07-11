namespace GymDiary.Persistence.MongoDB

open GymDiary.Persistence.MongoDB.Documents
open MongoDB.Driver

type IMongoContext =
    abstract member Users: IMongoCollection<UserDocument>
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
    abstract member ExerciseDefinitions: IMongoCollection<ExerciseDefinitionDocument>
    abstract member Routines: IMongoCollection<RoutineDocument>
    abstract member Workouts: IMongoCollection<WorkoutDocument>

type MongoContext(database: IMongoDatabase) =
    interface IMongoContext with
        member _.Users = database.GetCollection<UserDocument>("users")
        member _.ExerciseCategories = database.GetCollection<ExerciseCategoryDocument>("exerciseCategories")
        member _.ExerciseDefinitions = database.GetCollection<ExerciseDefinitionDocument>("exerciseDefinitions")
        member _.Routines = database.GetCollection<RoutineDocument>("routines")
        member _.Workouts = database.GetCollection<WorkoutDocument>("workouts")
