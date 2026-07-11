namespace GymDiary.Persistence.MongoDB.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.Routines
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open Microsoft.Extensions.Logging

type RoutineRepository
    (
        context: IMongoContext,
        mapper: IDocumentMapper<Routine, RoutineDocument>,
        logger: ILogger<RoutineRepository>
    ) =
    inherit OwnedEntityRepositoryBase<Routine, routineId, RoutineDocument>(context.Routines, mapper, logger)

    interface IRoutineRepository

