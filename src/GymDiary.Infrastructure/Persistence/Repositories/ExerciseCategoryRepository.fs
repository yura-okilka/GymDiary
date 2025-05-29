namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type ExerciseCategoryRepository(context: IMongoContext, mapper: IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>) =
    inherit OwnedEntityRepositoryBase<ExerciseCategory, ExerciseCategoryDocument>(context.ExerciseCategories, mapper)

    interface IExerciseCategoryRepository with
        member _.ExistsWithName name ownerId =
            let name = name.Value
            let ownerId = ownerId.Value

            // Consider using case-insensitive index for large collections.
            context.ExerciseCategories
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
