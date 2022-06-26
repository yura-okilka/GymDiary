namespace GymDiary.Api.DependencyInjection.Leaves

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type ExerciseCategoryRepository =
    { Create: ExerciseCategory -> PersistenceResult<Id<ExerciseCategory>>
      GetAll: Id<Sportsman> -> PersistenceResult<ExerciseCategory list>
      GetById: Id<Sportsman> -> Id<ExerciseCategory> -> PersistenceResult<ExerciseCategory>
      ExistWithName: Id<Sportsman> -> String50 -> PersistenceResult<bool>
      Update: ExerciseCategory -> PersistenceResult<unit>
      Delete: Id<ExerciseCategory> -> PersistenceResult<unit> }

type ExerciseTemplateRepository =
    { Create: ExerciseTemplate -> PersistenceResult<Id<ExerciseTemplate>>
      GetById: Id<Sportsman> -> Id<ExerciseTemplate> -> PersistenceResult<ExerciseTemplate>
      Update: ExerciseTemplate -> PersistenceResult<unit>
      Delete: Id<ExerciseTemplate> -> PersistenceResult<unit> }

type SportsmanRepository =
    { ExistWithId: Id<Sportsman> -> PersistenceResult<bool> }

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
