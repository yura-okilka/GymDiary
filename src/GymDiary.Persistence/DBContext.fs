namespace GymDiary.Persistence

open MongoDB.Driver

type DBContext =
    {
        ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
        Exercises: IMongoCollection<ExerciseDocument>
        Routines: IMongoCollection<RoutineDocument>
        WorkoutSessions: IMongoCollection<WorkoutSessionDocument>
        Sportsmen: IMongoCollection<SportsmanDocument>
    }

module DBContext =

    [<Literal>]
    let ExerciseCategories = "exerciseCategories"

    [<Literal>]
    let Exercises = "exercises"

    [<Literal>]
    let Routines = "routines"

    [<Literal>]
    let WorkoutSessions = "workoutSessions"

    [<Literal>]
    let Sportsmen = "sportsmen"

    let create (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)

        {
            ExerciseCategories = database.GetCollection<ExerciseCategoryDocument>(ExerciseCategories)
            Exercises = database.GetCollection<ExerciseDocument>(Exercises)
            Routines = database.GetCollection<RoutineDocument>(Routines)
            WorkoutSessions = database.GetCollection<WorkoutSessionDocument>(WorkoutSessions)
            Sportsmen = database.GetCollection<SportsmanDocument>(Sportsmen)
        }
