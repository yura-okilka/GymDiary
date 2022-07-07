namespace GymDiary.Core.Persistence

type ModifyEntityError = EntityNotFound of entity: string * id: string

type ModifyEntityResult = Async<Result<unit, ModifyEntityError>>
