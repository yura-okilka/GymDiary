namespace GymDiary.Api.DependencyInjection.Leaves

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type ExerciseCategoryRepository =
    { Create: ExerciseCategory -> Async<ExerciseCategoryId>
      GetAll: SportsmanId -> Async<ExerciseCategory list>
      GetById: SportsmanId -> ExerciseCategoryId -> Async<ExerciseCategory option>
      ExistWithName: SportsmanId -> String50 -> Async<bool>
      Update: ExerciseCategory -> ModifyEntityResult
      Delete: ExerciseCategoryId -> ModifyEntityResult }

type ExerciseTemplateRepository =
    { Create: ExerciseTemplate -> Async<ExerciseTemplateId>
      GetById: SportsmanId -> ExerciseTemplateId -> Async<ExerciseTemplate option>
      Update: ExerciseTemplate -> ModifyEntityResult
      Delete: ExerciseTemplateId -> ModifyEntityResult }

type SportsmanRepository =
    { ExistWithId: SportsmanId -> Async<bool> }

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

        let sportsmanRepository = { ExistWithId = SportsmanRepository.existWithId context.Sportsmen }

        { ExerciseCategory = exerciseCategoryRepository
          ExerciseTemplate = exerciseTemplateRepository
          Sportsman = sportsmanRepository }
