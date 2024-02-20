namespace GymDiary.Core.Persistence

open GymDiary.Core.Domain

type ModifyEntityError = EntityNotFound of entity: string * id: string

type ModifyEntityResult = Async<Result<unit, ModifyEntityError>>

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Async<ExerciseCategoryId>
    abstract member GetAll: SportsmanId -> Async<ExerciseCategory list>
    abstract member GetById: ExerciseCategoryId -> SportsmanId -> Async<ExerciseCategory option>
    abstract member ExistWithName: String50 -> SportsmanId -> Async<bool>
    abstract member Update: ExerciseCategory -> ModifyEntityResult
    abstract member Delete: ExerciseCategoryId -> ModifyEntityResult

type IExerciseRepository =
    abstract member Create: Exercise -> Async<ExerciseId>
    abstract member GetById: ExerciseId -> SportsmanId -> Async<Exercise option>
    abstract member Update: Exercise -> ModifyEntityResult
    abstract member Delete: ExerciseId -> ModifyEntityResult

type ISportsmanRepository =
    abstract member ExistWithId: SportsmanId -> Async<bool>
