namespace GymDiary.Persistence.MongoDB.Repositories

open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open MongoDB.Driver

type ExerciseCategoryRepository(context: IMongoContext, mapper: IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>) =
    inherit OwnedEntityRepositoryBase<ExerciseCategory, exerciseCategoryId, ExerciseCategoryDocument>(context.ExerciseCategories, mapper)
    interface IExerciseCategoryRepository with
        member _.ExistsWithName name ownerId =
            let name = name.Value
            let ownerId = %ownerId

            // Consider using case-insensitive index for large collections.
            context.ExerciseCategories
                .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                .AnyAsync()
