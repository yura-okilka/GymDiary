namespace GymDiary.Core.Persistence.Contracts

open System.Threading.Tasks

open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Domain.Errors

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Task<Result<ExerciseCategoryId, PersistenceError>>
    abstract member FindById: ExerciseCategoryId -> Task<Result<ExerciseCategory, PersistenceError>>
