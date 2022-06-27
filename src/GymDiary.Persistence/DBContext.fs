namespace GymDiary.Persistence

open MongoDB.Driver

type DBContext =
    { ExerciseCategories: IMongoCollection<ExerciseCategoryDocument>
      ExerciseTemplates: IMongoCollection<ExerciseTemplateDocument>
      WorkoutTemplates: IMongoCollection<WorkoutTemplateDocument>
      Workouts: IMongoCollection<WorkoutDocument>
      Sportsmen: IMongoCollection<SportsmanDocument> }

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
    let Sportsmen = "sportsmen"

    let create (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)

        { ExerciseCategories = database.GetCollection<ExerciseCategoryDocument>(ExerciseCategories)
          ExerciseTemplates = database.GetCollection<ExerciseTemplateDocument>(ExerciseTemplates)
          WorkoutTemplates = database.GetCollection<WorkoutTemplateDocument>(WorkoutTemplates)
          Workouts = database.GetCollection<WorkoutDocument>(Workouts)
          Sportsmen = database.GetCollection<SportsmanDocument>(Sportsmen) }
