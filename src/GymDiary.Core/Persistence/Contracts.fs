namespace GymDiary.Core.Persistence.ExerciseCategory

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes

type CreateExerciseCategoryInDB = ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>
type GetExerciseCategoryByIdFromDB = ExerciseCategoryId -> Async<Result<ExerciseCategory, PersistenceError>>
type ExerciseCategoryWithNameExistsInDB = string -> Async<Result<bool, PersistenceError>>
type UpdateExerciseCategoryInDB = ExerciseCategory -> Async<Result<unit, PersistenceError>>
type DeleteExerciseCategoryFromDB = ExerciseCategoryId -> Async<Result<unit, PersistenceError>>

type IExerciseCategoryRepository =
    abstract member Create: CreateExerciseCategoryInDB
    abstract member GetById: GetExerciseCategoryByIdFromDB
    abstract member ExistWithName: ExerciseCategoryWithNameExistsInDB
    abstract member Update: UpdateExerciseCategoryInDB
    abstract member Delete: DeleteExerciseCategoryFromDB

namespace GymDiary.Core.Persistence.ExerciseTemplate

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes

type CreateExerciseTemplateInDB = ExerciseTemplate -> Async<Result<ExerciseTemplateId, PersistenceError>>
type GetExerciseTemplateByIdFromDB = ExerciseTemplateId -> Async<Result<ExerciseTemplate, PersistenceError>>
type UpdateExerciseTemplateInDB = ExerciseTemplate -> Async<Result<unit, PersistenceError>>
type DeleteExerciseTemplateFromDB = ExerciseTemplateId -> Async<Result<unit, PersistenceError>>

type IExerciseTemplateRepository =
    abstract member Create: CreateExerciseTemplateInDB
    abstract member GetById: GetExerciseTemplateByIdFromDB
    abstract member Update: UpdateExerciseTemplateInDB
    abstract member Delete: DeleteExerciseTemplateFromDB

namespace GymDiary.Core.Persistence

open GymDiary.Core.Persistence.ExerciseCategory
open GymDiary.Core.Persistence.ExerciseTemplate

type IPersistenceCompositionRoot =
    abstract member ExerciseCategoryRepository: IExerciseCategoryRepository
    abstract member ExerciseTemplateRepository: IExerciseTemplateRepository
