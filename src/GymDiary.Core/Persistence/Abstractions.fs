namespace GymDiary.Core.Persistence

open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IIdGenerator =
    abstract member GenerateId<'T> : unit -> Id<'T>

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Async<ExerciseCategoryId>
    abstract member GetAll: UserId -> Async<ExerciseCategory list>
    abstract member GetById: ExerciseCategoryId -> UserId -> Async<ExerciseCategory option>
    abstract member ExistWithName: String50 -> UserId -> Async<bool>
    abstract member Update: ExerciseCategory -> Async<UpdateEntityResult>
    abstract member Delete: ExerciseCategoryId -> Async<unit>

type IExerciseRepository =
    abstract member Create: ExerciseDefinition -> Async<ExerciseDefinitionId>
    abstract member GetById: ExerciseDefinitionId -> UserId -> Async<ExerciseDefinition option>
    abstract member Update: ExerciseDefinition -> Async<UpdateEntityResult>
    abstract member Delete: ExerciseDefinitionId -> Async<unit>

type IUserRepository =
    abstract member Create: User -> Async<UserId>
    abstract member ExistWithId: UserId -> Async<bool>
    abstract member ExistWithEmail: EmailAddress -> Async<bool>
