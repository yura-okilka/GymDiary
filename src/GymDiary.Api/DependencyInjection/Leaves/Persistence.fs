namespace GymDiary.Api.DependencyInjection.Leaves

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence
open GymDiary.Persistence.Repositories

open MongoDB.Driver

type ExerciseCategoryRepository =
    { Create: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>
      GetAll: unit -> Async<Result<ExerciseCategory list, PersistenceError>>
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
            { Create = fun entity -> ExerciseCategoryRepository.create context.ExerciseCategories entity
              GetAll = fun _ -> ExerciseCategoryRepository.getAll context.ExerciseCategories
              GetById = fun id -> ExerciseCategoryRepository.getById context.ExerciseCategories id
              ExistWithName = fun name -> ExerciseCategoryRepository.existWithName context.ExerciseCategories name
              Update = fun entity -> ExerciseCategoryRepository.update context.ExerciseCategories entity
              Delete = fun id -> ExerciseCategoryRepository.delete context.ExerciseCategories id }

        let exerciseTemplateRepository =
            { Create = fun entity -> ExerciseTemplateRepository.create context.ExerciseTemplates entity
              GetById = fun id -> ExerciseTemplateRepository.getById context.ExerciseTemplates id
              Update = fun entity -> ExerciseTemplateRepository.update context.ExerciseTemplates entity
              Delete = fun id -> ExerciseTemplateRepository.delete context.ExerciseTemplates id }

        let sportsmanRepository =
            { ExistWithId = fun id -> SportsmanRepository.existWithId context.Sportsmans id }

        { ExerciseCategory = exerciseCategoryRepository
          ExerciseTemplate = exerciseTemplateRepository
          Sportsman = sportsmanRepository }
