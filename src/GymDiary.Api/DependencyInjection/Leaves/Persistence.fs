namespace GymDiary.Api.DependencyInjection.Leaves

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type ExerciseCategoryRepository = {
    Create: ExerciseCategory -> Async<ExerciseCategoryId>
    GetAll: SportsmanId -> Async<ExerciseCategory list>
    GetById: SportsmanId -> ExerciseCategoryId -> Async<ExerciseCategory option>
    ExistWithName: SportsmanId -> String50 -> Async<bool>
    Update: ExerciseCategory -> ModifyEntityResult
    Delete: ExerciseCategoryId -> ModifyEntityResult
}

type ExerciseRepository = {
    Create: Exercise -> Async<ExerciseId>
    GetById: SportsmanId -> ExerciseId -> Async<Exercise option>
    Update: Exercise -> ModifyEntityResult
    Delete: ExerciseId -> ModifyEntityResult
}

type SportsmanRepository = {
    ExistWithId: SportsmanId -> Async<bool>
}

type Persistence = {
    ExerciseCategory: ExerciseCategoryRepository
    Exercise: ExerciseRepository
    Sportsman: SportsmanRepository
}

module Persistence =

    let compose (client: IMongoClient) (dbName: string) =
        let context = DBContext.create client dbName

        {
            ExerciseCategory = {
                Create = ExerciseCategoryRepository.create context.ExerciseCategories
                GetAll = ExerciseCategoryRepository.getAll context.ExerciseCategories
                GetById = ExerciseCategoryRepository.getById context.ExerciseCategories
                ExistWithName = ExerciseCategoryRepository.existWithName context.ExerciseCategories
                Update = ExerciseCategoryRepository.update context.ExerciseCategories
                Delete = ExerciseCategoryRepository.delete context.ExerciseCategories
            }
            Exercise = {
                Create = ExerciseRepository.create context.Exercises
                GetById = ExerciseRepository.getById context.Exercises
                Update = ExerciseRepository.update context.Exercises
                Delete = ExerciseRepository.delete context.Exercises
            }
            Sportsman = {
                ExistWithId = SportsmanRepository.existWithId context.Sportsmen
            }
        }
