namespace GymDiary.Core.Persistence.ExerciseCategory

open System.Threading.Tasks

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes

type CreateExerciseCategoryInDB = ExerciseCategory -> Task<Result<ExerciseCategoryId, PersistenceError>>
type GetExerciseCategoryByIdFromDB = ExerciseCategoryId -> Task<Result<ExerciseCategory, PersistenceError>>
type FindExerciseCategoryByNameInDB = string -> Task<Result<ExerciseCategory option, PersistenceError>>
type UpdateExerciseCategoryInDB = ExerciseCategory -> Task<Result<unit, PersistenceError>>
type DeleteExerciseCategoryFromDB = ExerciseCategoryId -> Task<Result<unit, PersistenceError>>

type IExerciseCategoryRepository =
    abstract member Create: CreateExerciseCategoryInDB
    abstract member GetById: GetExerciseCategoryByIdFromDB
    abstract member FindByName: FindExerciseCategoryByNameInDB
    abstract member Update: UpdateExerciseCategoryInDB
    abstract member Delete: DeleteExerciseCategoryFromDB

namespace GymDiary.Core.Persistence.ExerciseTemplate

open System.Threading.Tasks

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes

type CreateExerciseTemplateInDB = ExerciseTemplate -> Task<Result<ExerciseTemplateId, PersistenceError>>
type GetExerciseTemplateByIdFromDB = ExerciseTemplateId -> Task<Result<ExerciseTemplate, PersistenceError>>
type UpdateExerciseTemplateInDB = ExerciseTemplate -> Task<Result<unit, PersistenceError>>
type DeleteExerciseTemplateFromDB = ExerciseTemplateId -> Task<Result<unit, PersistenceError>>

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
