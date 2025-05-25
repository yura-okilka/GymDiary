namespace GymDiary.Infrastructure.Persistence

open GymDiary.Infrastructure.Persistence.Documents
open MongoDB.Driver

type IMongoContext =
    abstract member Users: IMongoCollection<UserDocument>
    abstract member UsersV2: IMongoCollection<UserDocumentV2>
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
    abstract member ExerciseCategoriesV2: IMongoCollection<ExerciseCategoryDocumentV2>
    abstract member ExerciseDefinitions: IMongoCollection<ExerciseDefinitionDocument>
    abstract member Routines: IMongoCollection<RoutineDocument>
    abstract member Workouts: IMongoCollection<WorkoutDocument>

type MongoContext(mongoClient: IMongoClient, mongoSettings: MongoSettings) =
    member private _.GetCollection collection =
        mongoClient
            .GetDatabase(mongoSettings.Database)
            .GetCollection<'TDocument>(collection)

    interface IMongoContext with
        member c.Users = c.GetCollection<UserDocument>("users")
        member c.UsersV2 = c.GetCollection<UserDocumentV2>("usersV2")
        member c.ExerciseCategories = c.GetCollection<ExerciseCategoryDocument>("exerciseCategories")
        member c.ExerciseCategoriesV2 = c.GetCollection<ExerciseCategoryDocumentV2>("exerciseCategoriesV2")
        member c.ExerciseDefinitions = c.GetCollection<ExerciseDefinitionDocument>("exerciseDefinitions")
        member c.Routines = c.GetCollection<RoutineDocument>("routines")
        member c.Workouts = c.GetCollection<WorkoutDocument>("workouts")
