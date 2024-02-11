namespace GymDiary.Persistence

open MongoDB.Driver

type DBContext = {
    ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
    Exercises: IMongoCollection<ExerciseDocument>
    Routines: IMongoCollection<RoutineDocument>
    WorkoutSessions: IMongoCollection<WorkoutSessionDocument>
    Sportsmen: IMongoCollection<SportsmanDocument>
} with

    static member create (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)

        {
            ExerciseCategories = database.GetCollection<ExerciseCategoryDocument>("exerciseCategories")
            Exercises = database.GetCollection<ExerciseDocument>("exercises")
            Routines = database.GetCollection<RoutineDocument>("routines")
            WorkoutSessions = database.GetCollection<WorkoutSessionDocument>("workoutSessions")
            Sportsmen = database.GetCollection<SportsmanDocument>("sportsmen")
        }
