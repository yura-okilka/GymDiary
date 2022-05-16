namespace GymDiary.Api.DependencyInjection.Leaves

open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type ExerciseCategoryRepository =
    { Create: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>
      GetAll: SportsmanId -> Async<Result<ExerciseCategory list, PersistenceError>>
      GetById: ExerciseCategoryId -> Async<Result<ExerciseCategory, PersistenceError>>
      ExistWithName: String50 -> Async<Result<bool, PersistenceError>>
      Update: ExerciseCategory -> Async<Result<unit, PersistenceError>>
      Delete: ExerciseCategoryId -> Async<Result<unit, PersistenceError>> }

type ExerciseTemplateRepository =
    { Create: ExerciseTemplate -> Async<Result<ExerciseTemplateId, PersistenceError>>
      GetById: ExerciseTemplateId -> Async<Result<ExerciseTemplate, PersistenceError>>
      Update: ExerciseTemplate -> Async<Result<unit, PersistenceError>>
      Delete: ExerciseTemplateId -> Async<Result<unit, PersistenceError>> }

type SportsmanRepository =
    { ExistWithId: SportsmanId -> Async<Result<bool, PersistenceError>> }

type Persistence =
    { ExerciseCategory: ExerciseCategoryRepository
      ExerciseTemplate: ExerciseTemplateRepository
      Sportsman: SportsmanRepository }

module Persistence =

    let compose (client: IMongoClient) (dbName: string) =
        let context = DBContext.create client dbName

        let exerciseCategoryRepository =
            { Create = ExerciseCategoryRepository.create context.ExerciseCategories
              GetAll = ExerciseCategoryRepository.getAll context.ExerciseCategories
              GetById = ExerciseCategoryRepository.getById context.ExerciseCategories
              ExistWithName = ExerciseCategoryRepository.existWithName context.ExerciseCategories
              Update = ExerciseCategoryRepository.update context.ExerciseCategories
              Delete = ExerciseCategoryRepository.delete context.ExerciseCategories }

        let exerciseTemplateRepository =
            { Create = ExerciseTemplateRepository.create context.ExerciseTemplates
              GetById = ExerciseTemplateRepository.getById context.ExerciseTemplates
              Update = ExerciseTemplateRepository.update context.ExerciseTemplates
              Delete = ExerciseTemplateRepository.delete context.ExerciseTemplates }

        let sportsmanRepository = { ExistWithId = SportsmanRepository.existWithId context.Sportsmans }

        { ExerciseCategory = exerciseCategoryRepository
          ExerciseTemplate = exerciseTemplateRepository
          Sportsman = sportsmanRepository }
