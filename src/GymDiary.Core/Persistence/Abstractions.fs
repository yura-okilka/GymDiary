namespace GymDiary.Core.Persistence

open GymDiary.Core.Domain

type UpdateEntityError = EntityNotFound of entity: string * id: string

type UpdateEntityResult = Async<Result<unit, UpdateEntityError>>

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Async<ExerciseCategoryId>
    abstract member GetAll: SportsmanId -> Async<ExerciseCategory list>
    abstract member GetById: ExerciseCategoryId -> SportsmanId -> Async<ExerciseCategory option>
    abstract member ExistWithName: String50 -> SportsmanId -> Async<bool>
    abstract member Update: ExerciseCategory -> UpdateEntityResult
    abstract member Delete: ExerciseCategoryId -> Async<unit>

type IExerciseRepository =
    abstract member Create: Exercise -> Async<ExerciseId>
    abstract member GetById: ExerciseId -> SportsmanId -> Async<Exercise option>
    abstract member Update: Exercise -> UpdateEntityResult
    abstract member Delete: ExerciseId -> Async<unit>

type ISportsmanRepository =
    abstract member Create: Sportsman -> Async<SportsmanId>
    abstract member ExistWithId: SportsmanId -> Async<bool>
    abstract member ExistWithEmail: EmailAddress -> Async<bool>
