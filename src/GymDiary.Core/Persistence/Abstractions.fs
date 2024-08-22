namespace GymDiary.Core.Persistence

open System.Threading.Tasks
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IIdProvider =
    abstract member GenerateId<'T> : unit -> Id<'T>

type IExerciseCategoryRepository =
    abstract member Create: ExerciseCategory -> Task<unit>
    abstract member Update: ExerciseCategory -> Task<UpdateEntityResult>
    abstract member Delete: ExerciseCategoryId -> Task<unit>
    abstract member Get: ExerciseCategoryId -> UserId -> Task<ExerciseCategory option>
    abstract member GetAll: UserId -> Task<ExerciseCategory list>
    abstract member ExistWithName: String50 -> UserId -> Task<bool>

type IExerciseDefinitionRepository =
    abstract member Create: ExerciseDefinition -> Task<unit>
    abstract member Update: ExerciseDefinition -> Task<UpdateEntityResult>
    abstract member Delete: ExerciseDefinitionId -> Task<unit>
    abstract member Get: ExerciseDefinitionId -> UserId -> Task<ExerciseDefinition option>

type IUserRepository =
    abstract member Create: User -> Task<unit>
    abstract member ExistWithId: UserId -> Task<bool>
    abstract member ExistWithEmail: EmailAddress -> Task<bool>
