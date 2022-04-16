namespace GymDiary.Core.Persistence.ExerciseCategory

open System.Threading.Tasks

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes

type CreateExerciseCategoryInDB = ExerciseCategory -> Task<Result<ExerciseCategoryId, PersistenceError>>
type GetExerciseCategoryByIdFromDB = ExerciseCategoryId -> Task<Result<ExerciseCategory, PersistenceError>>
type UpdateExerciseCategoryInDB = ExerciseCategory -> Task<Result<unit, PersistenceError>>
type DeleteExerciseCategoryFromDB = ExerciseCategoryId -> Task<Result<unit, PersistenceError>>

type IExerciseCategoryRepository =
    abstract member Create: CreateExerciseCategoryInDB
    abstract member GetById: GetExerciseCategoryByIdFromDB
    abstract member Update: UpdateExerciseCategoryInDB
    abstract member Delete: DeleteExerciseCategoryFromDB

namespace GymDiary.Core.Persistence

open GymDiary.Core.Persistence.ExerciseCategory

type IPersistenceCompositionRoot =
    abstract member ExerciseCategoryRepository: IExerciseCategoryRepository
