namespace GymDiary.Persistence.MongoDB.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping

type ExerciseDefinitionRepository(context: IMongoContext, mapper: IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>) =
    inherit OwnedEntityRepositoryBase<ExerciseDefinition, exerciseDefinitionId, ExerciseDefinitionDocument>(context.ExerciseDefinitions, mapper)
    interface IExerciseDefinitionRepository
