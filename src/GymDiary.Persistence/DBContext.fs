namespace GymDiary.Persistence

open GymDiary.Persistence.Dtos

open MongoDB.Driver

type IDBContext =
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDto>
    abstract member Workouts: IMongoCollection<WorkoutDto>
    abstract member Sportsmans: IMongoCollection<SportsmanDto>

module DBContext =

    [<Literal>]
    let ExerciseCategories = "exerciseCategories"

    [<Literal>]
    let Workouts = "workouts"

    [<Literal>]
    let Sportsmans = "sportsmans"

    let createContext (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)
        let exerciseCategories = database.GetCollection<ExerciseCategoryDto>(ExerciseCategories)
        let workouts = database.GetCollection<WorkoutDto>(Workouts)
        let sportsmans = database.GetCollection<SportsmanDto>(Sportsmans)

        { new IDBContext with
            member _.ExerciseCategories = exerciseCategories
            member _.Workouts = workouts
            member _.Sportsmans = sportsmans }
