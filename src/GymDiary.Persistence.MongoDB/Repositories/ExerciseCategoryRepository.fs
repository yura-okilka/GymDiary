namespace GymDiary.Persistence.MongoDB.Repositories

open FSharp.UMX
open GymDiary.Application.Persistence
open GymDiary.Domain.ExerciseCategories
open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping
open Microsoft.Extensions.Logging
open MongoDB.Driver

type ExerciseCategoryRepository
    (
        context: IMongoContext,
        mapper: IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument>,
        logger: ILogger<ExerciseCategoryRepository>
    ) =
    inherit
        OwnedEntityRepositoryBase<ExerciseCategory, exerciseCategoryId, ExerciseCategoryDocument>(
            context.ExerciseCategories,
            mapper,
            logger
        )

    interface IExerciseCategoryRepository with
        member _.ExistsWithName name ownerId = async {
            let name = name.Value
            let ownerId = %ownerId

            // Consider using case-insensitive index for large collections.
            let! exists =
                context.ExerciseCategories
                    .Find(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId)
                    .AnyAsync()
                |> Async.AwaitTask

            logger.LogDebug("Checked ExerciseCategory name {Name} existence for owner {OwnerId}: {Exists}", name, ownerId, exists)
            return exists
        }
