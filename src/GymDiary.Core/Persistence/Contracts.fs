namespace GymDiary.Core.Persistence

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>
    abstract member GetById: ExerciseCategoryId -> Async<Result<ExerciseCategory, PersistenceError>>
    abstract member ExistWithName: String50 -> Async<Result<bool, PersistenceError>>
    abstract member Update: ExerciseCategory -> Async<Result<unit, PersistenceError>>
    abstract member Delete: ExerciseCategoryId -> Async<Result<unit, PersistenceError>>

type IExerciseTemplateRepository =
    abstract member Create: ExerciseTemplate -> Async<Result<ExerciseTemplateId, PersistenceError>>
    abstract member GetById: ExerciseTemplateId -> Async<Result<ExerciseTemplate, PersistenceError>>
    abstract member Update: ExerciseTemplate -> Async<Result<unit, PersistenceError>>
    abstract member Delete: ExerciseTemplateId -> Async<Result<unit, PersistenceError>>

type ISportsmanRepository =
    abstract member ExistWithId: SportsmanId -> Async<Result<bool, PersistenceError>>

type IPersistenceCompositionRoot =
    abstract member ExerciseCategoryRepository: IExerciseCategoryRepository
    abstract member ExerciseTemplateRepository: IExerciseTemplateRepository
    abstract member SportsmanRepository: ISportsmanRepository
