namespace GymDiary.Infrastructure.Persistence.Repositories

open GymDiary.Application.Persistence
open GymDiary.Application.Time
open GymDiary.Domain.ExerciseCategories
open GymDiary.Infrastructure.Persistence
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Infrastructure.Persistence.Mapping
open MongoDB.Driver

type ExerciseCategoryRepositoryV2
    (context: IMongoContext, mapper: IDocumentMapper<ExerciseCategory, ExerciseCategoryDocumentV2>, clock: IClock) =
    inherit OwnedEntityRepositoryBase<ExerciseCategory, ExerciseCategoryDocumentV2>(mapper, clock)
    override this.Collection = context.ExerciseCategoriesV2

    interface IExerciseCategoryRepository with

        member this.ExistsWithName name ownerId =
            let name = name.Value
            let ownerId = ownerId.Value

            // Consider using case-insensitive index for large collections.
            this.Collection
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
