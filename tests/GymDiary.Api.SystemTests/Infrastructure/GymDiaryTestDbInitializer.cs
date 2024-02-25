using GymDiary.DbMigrations;

using MongoDB.Bson;
using MongoDB.Driver;

namespace GymDiary.Api.SystemTests.Infrastructure;

public class GymDiaryTestDbInitializer(IMongoDatabase database, MongoMigrator migrator)
{
    public async Task Initialize()
    {
        await migrator.Upgrade();
        await ClearData();
    }

    public async Task ClearData()
    {
        var collections = await database.ListCollectionNames().ToListAsync();

        foreach (var collection in collections)
        {
            await database.GetCollection<BsonDocument>(collection).DeleteManyAsync(_ => true);
        }
    }
}
