namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type ExerciseCategoryRepositoryV2(context: IMongoContext, mapper: IDocumentMapper<ExerciseCategory, ExerciseCategoryDocumentV2>) =
    inherit OwnedEntityRepositoryBase<ExerciseCategory, ExerciseCategoryDocumentV2>(context.ExerciseCategoriesV2, mapper)

    interface IExerciseCategoryRepository with
        member _.ExistsWithName name ownerId =
            let name = name.Value
            let ownerId = ownerId.Value

            // Consider using case-insensitive index for large collections.
            context.ExerciseCategoriesV2
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
