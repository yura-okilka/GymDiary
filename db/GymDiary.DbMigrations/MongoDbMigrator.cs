using Microsoft.Extensions.Logging;

using MongoDB.Driver;

namespace GymDiary.DbMigrations;

public static class MongoCollections
{
    public const string ExerciseCategories = "exerciseCategories";
    public const string Exercises = "exercises";
    public const string Routines = "routines";
    public const string WorkoutSessions = "workoutSessions";
    public const string Sportsmen = "sportsmen";

    public static IEnumerable<string> All => [ExerciseCategories, Exercises, Routines, WorkoutSessions, Sportsmen];
}

/// <summary>
///     Simple MongoDB migrator to initialize an empty database with collections, indexes, etc.
/// </summary>
/// <remarks>
///     It does not support migration scripts and journaling yet.
/// </remarks>
public class MongoDbMigrator(string database, IMongoClient mongoClient, ILogger<MongoDbMigrator> logger)
    : IDbMigrator
{
    private IMongoDatabase Db => mongoClient.GetDatabase(database);

    public async Task Upgrade()
    {
        try
        {
            logger.LogInformation("Upgrading '{database}' database", database);

            foreach (var collection in MongoCollections.All)
            {
                await CreateCollectionWithDefaultSettings(collection);
            }

            logger.LogInformation("Upgrade completed successfully");
        }
        catch (Exception ex)
        {
            var upgradeEx = new DbUpgradeException(database, ex);
            logger.LogError(upgradeEx, "Failed to upgrade '{database}' database", database);

            throw upgradeEx;
        }
    }

    private Task CreateCollectionWithDefaultSettings(string collection) => Db.CreateCollectionAsync(collection);
}
