namespace GymDiary.Application.Persistence

open System.Threading.Tasks
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IEntityRepository<'TEntity, 'TEntityId> =
    abstract member Create: 'TEntity -> Task<unit>
    abstract member Update: 'TEntity -> Task<UpdateEntityResult>
    abstract member Delete: 'TEntityId -> Task<unit>
    abstract member Get: 'TEntityId -> Task<'TEntity option>
    abstract member ExistsWithId: 'TEntityId -> Task<bool>

type IOwnedEntityRepository<'TEntity, 'TEntityId> =
    inherit IEntityRepository<'TEntity, 'TEntityId>
    abstract member GetOneByOwner: 'TEntityId -> UserId -> Task<'TEntity option>
    abstract member GetAllByOwner: UserId -> Task<'TEntity list>
    abstract member DeleteByOwner: 'TEntityId -> UserId -> Task<bool>

type IUserRepository =
    inherit IEntityRepository<User, UserId>
    abstract member ExistsWithEmail: EmailAddress -> Task<bool>

type IExerciseCategoryRepository =
    inherit IOwnedEntityRepository<ExerciseCategory, ExerciseCategoryId>
    abstract member ExistsWithName: String50 -> UserId -> Task<bool>

type IExerciseDefinitionRepository =
    inherit IOwnedEntityRepository<ExerciseDefinition, ExerciseDefinitionId>

type IRoutineRepository =
    inherit IOwnedEntityRepository<Routine, RoutineId>
