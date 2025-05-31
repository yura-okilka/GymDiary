namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping

type ExerciseDefinitionRepository(context: IMongoContext, mapper: IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>) =
    inherit OwnedEntityRepositoryBase<ExerciseDefinition, ExerciseDefinitionDocument>(context.ExerciseDefinitions, mapper)
    interface IExerciseDefinitionRepository
