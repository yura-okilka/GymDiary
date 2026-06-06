namespace GymDiary.Persistence.MongoDB.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Routines
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping

type RoutineRepository(context: IMongoContext, mapper: IDocumentMapper<Routine, RoutineDocument>) =
    inherit OwnedEntityRepositoryBase<Routine, RoutineDocument>(context.Routines, mapper)
    interface IRoutineRepository
