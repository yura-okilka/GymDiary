namespace GymDiary.Application.Persistence

open System.Threading.Tasks
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

type IEntityIdProvider =
    abstract member GenerateId: unit -> Id<'T>
    abstract member TryParse: string -> Id<'T> option
    abstract member TryParseResult: string -> Result<Id<'T>, string>

type UpdateEntityError = EntityNotFound of entity: string * id: string
type UpdateEntityResult = Result<unit, UpdateEntityError>

type IEntityRepository<'TEntity> =
    abstract member Create: 'TEntity -> Task<unit>
    abstract member Update: 'TEntity -> Task<UpdateEntityResult>
    abstract member Delete: Id<'TEntity> -> Task<unit>
    abstract member Get: Id<'TEntity> -> Task<'TEntity option>
    abstract member ExistsWithId: Id<'TEntity> -> Task<bool>

type IOwnedEntityRepository<'TEntity> =
    inherit IEntityRepository<'TEntity>
    abstract member GetOneByOwner: Id<'TEntity> -> UserId -> Task<'TEntity option>
    abstract member GetAllByOwner: UserId -> Task<'TEntity list>

type IUserRepository =
    inherit IEntityRepository<User>
    abstract member ExistsWithEmail: EmailAddress -> Task<bool>

type IExerciseCategoryRepository =
    inherit IOwnedEntityRepository<ExerciseCategory>
    abstract member ExistsWithName: String50 -> UserId -> Task<bool>

type IExerciseDefinitionRepository =
    inherit IOwnedEntityRepository<ExerciseDefinition>

type IRoutineRepository =
    inherit IOwnedEntityRepository<Routine>
