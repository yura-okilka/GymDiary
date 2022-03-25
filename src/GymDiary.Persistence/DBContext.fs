namespace GymDiary.Persistence

open GymDiary.Persistence.Dtos

open MongoDB.Driver

type IDBContext =
    abstract member ExerciseCategories: IMongoCollection<ExerciseCategoryDto>

module DBContext =

    [<Literal>]
    let ExerciseCategories = "exerciseCategories"

    let compose (client: IMongoClient) (dbName: string) =
        let database = client.GetDatabase(dbName)
        let exerciseCategories = database.GetCollection<ExerciseCategoryDto>(ExerciseCategories)

        { new IDBContext with
            member _.ExerciseCategories = exerciseCategories }
