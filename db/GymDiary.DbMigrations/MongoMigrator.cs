using Microsoft.Extensions.Logging;

using MongoDB.Driver;

namespace GymDiary.DbMigrations;

/// <summary>
///     Simple MongoDB migrator to initialize an empty database with collections, indexes, etc.
/// </summary>
/// <remarks>
///     It does not support migration scripts and journaling yet.
/// </remarks>
public class MongoMigrator(IMongoDatabase database, ILogger<MongoMigrator> logger) : IDbMigrator
{
    private string DatabaseName => database.DatabaseNamespace.DatabaseName;

    public async Task Upgrade()
    {
        try
        {
            logger.LogInformation("Upgrading '{database}' database", DatabaseName);

            var existingCollections = await database.ListCollectionNames().ToListAsync();

            foreach (var collection in MongoCollections.All.Except(existingCollections))
            {
                await CreateCollectionWithDefaultSettings(collection);
            }

            logger.LogInformation("Upgrade completed successfully");
        }
        catch (Exception ex)
        {
            var upgradeEx = new DbUpgradeException(DatabaseName, ex);
            logger.LogError(upgradeEx, "Failed to upgrade '{database}' database", DatabaseName);

            throw upgradeEx;
        }
    }

    private async Task CreateCollectionWithDefaultSettings(string collection)
    {
        await database.CreateCollectionAsync(collection);

        logger.LogInformation($"Created '{collection}' collection with default settings");
    }
}
