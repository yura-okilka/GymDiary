namespace GymDiary.Persistence

open MongoDB.Driver

type DBContext =
    { ExerciseCategories: IMongoCollection<ExerciseCategoryDto>
      ExerciseTemplates: IMongoCollection<ExerciseTemplateDto>
      WorkoutTemplates: IMongoCollection<WorkoutTemplateDto>
      Workouts: IMongoCollection<WorkoutDto>
      Sportsmans: IMongoCollection<SportsmanDto> }

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

    let create (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)

        { ExerciseCategories = database.GetCollection<ExerciseCategoryDto>(ExerciseCategories)
          ExerciseTemplates = database.GetCollection<ExerciseTemplateDto>(ExerciseTemplates)
          WorkoutTemplates = database.GetCollection<WorkoutTemplateDto>(WorkoutTemplates)
          Workouts = database.GetCollection<WorkoutDto>(Workouts)
          Sportsmans = database.GetCollection<SportsmanDto>(Sportsmans) }
