namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Routines
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping

type RoutineRepository(context: IMongoContext, mapper: IDocumentMapper<Routine, RoutineDocument>) =
    inherit OwnedEntityRepositoryBase<Routine, RoutineDocument>(context.Routines, mapper)
    interface IRoutineRepository
