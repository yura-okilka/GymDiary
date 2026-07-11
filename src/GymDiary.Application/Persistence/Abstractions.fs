namespace GymDiary.Application.Persistence

open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IEntityRepository<'TEntity, 'TEntityId> =
    abstract member Create: 'TEntity -> Async<unit>
    abstract member Update: 'TEntity -> Async<UpdateEntityResult>
    abstract member Delete: 'TEntityId -> Async<unit>
    abstract member Get: 'TEntityId -> Async<'TEntity option>
    abstract member ExistsWithId: 'TEntityId -> Async<bool>

type IOwnedEntityRepository<'TEntity, 'TEntityId> =
    inherit IEntityRepository<'TEntity, 'TEntityId>
    abstract member GetOneByOwner: 'TEntityId -> UserId -> Async<'TEntity option>
    abstract member GetAllByOwner: UserId -> Async<'TEntity list>
    abstract member DeleteByOwner: 'TEntityId -> UserId -> Async<bool>

type IUserRepository =
    inherit IEntityRepository<User, UserId>
    abstract member ExistsWithEmail: EmailAddress -> Async<bool>

type IExerciseCategoryRepository =
    inherit IOwnedEntityRepository<ExerciseCategory, ExerciseCategoryId>
    abstract member ExistsWithName: String50 -> UserId -> Async<bool>

type IExerciseDefinitionRepository =
    inherit IOwnedEntityRepository<ExerciseDefinition, ExerciseDefinitionId>

type IRoutineRepository =
    inherit IOwnedEntityRepository<Routine, RoutineId>
