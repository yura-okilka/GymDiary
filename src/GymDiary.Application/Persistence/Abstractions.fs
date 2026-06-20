namespace GymDiary.Application.Persistence

open System.Threading.Tasks
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IEntityRepository<'TEntity, 'TId> =
    abstract member Create: 'TEntity -> Task<unit>
    abstract member Update: 'TEntity -> Task<UpdateEntityResult>
    abstract member Delete: 'TId -> Task<unit>
    abstract member Get: 'TId -> Task<'TEntity option>
    abstract member ExistsWithId: 'TId -> Task<bool>

type IOwnedEntityRepository<'TEntity, 'TId> =
    inherit IEntityRepository<'TEntity, 'TId>
    abstract member GetOneByOwner: 'TId -> UserId -> Task<'TEntity option>
    abstract member GetAllByOwner: UserId -> Task<'TEntity list>
    abstract member DeleteByOwner: 'TId -> UserId -> Task<bool>

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
