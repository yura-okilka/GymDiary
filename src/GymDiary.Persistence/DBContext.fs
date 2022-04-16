namespace GymDiary.Persistence

open GymDiary.Persistence.Dtos

open MongoDB.Driver

type IDBContext =
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDto>
    abstract member ExerciseTemplates: IMongoCollection<ExerciseTemplateDto>
    abstract member WorkoutTemplates: IMongoCollection<WorkoutTemplateDto>
    abstract member Workouts: IMongoCollection<WorkoutDto>
    abstract member Sportsmans: IMongoCollection<SportsmanDto>

module DBContext =

    [<Literal>]
    let ExerciseCategories = "exerciseCategories"

    [<Literal>]
    let ExerciseTemplates = "exerciseTemplates"

    [<Literal>]
    let WorkoutTemplates = "workoutTemplates"

    [<Literal>]
    let Workouts = "workouts"

    [<Literal>]
    let Sportsmans = "sportsmans"

    let createContext (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)
        let exerciseCategories = database.GetCollection<ExerciseCategoryDto>(ExerciseCategories)
        let exerciseTemplates = database.GetCollection<ExerciseTemplateDto>(ExerciseTemplates)
        let workoutTemplates = database.GetCollection<WorkoutTemplateDto>(WorkoutTemplates)
        let workouts = database.GetCollection<WorkoutDto>(Workouts)
        let sportsmans = database.GetCollection<SportsmanDto>(Sportsmans)

        { new IDBContext with
            member _.ExerciseCategories = exerciseCategories
            member _.ExerciseTemplates = exerciseTemplates
            member _.WorkoutTemplates = workoutTemplates
            member _.Workouts = workouts
            member _.Sportsmans = sportsmans }
