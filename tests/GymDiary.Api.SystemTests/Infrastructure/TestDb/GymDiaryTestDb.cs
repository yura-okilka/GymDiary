using GymDiary.DbMigrations;

using LightBDD.Core.Execution;

using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Driver;

namespace GymDiary.Api.SystemTests.Infrastructure.TestDb;

public record GymDiaryTestDbSettings(
    string Database,
    DbHostingType HostingType,
    ManagedMongoDbContainerSettings? ManagedDb,
    ExternalMongoDbContainerSettings? ExternalDb
);

public enum DbHostingType
{
    ManagedDb = 1,
    ExternalDb
}

public class GymDiaryTestDb : IDisposable, IGlobalResourceSetUp, IGymDiaryDb
{
    private readonly GymDiaryTestDbSettings _settings;
    private readonly IDbContainer _container;
    private readonly Action<GymDiaryTestDb>? _onSetUpExecuted;
    private GymDiaryTestDbInitializer? _dbInitializer;

    public GymDiaryTestDb(GymDiaryTestDbSettings settings, Action<GymDiaryTestDb>? onSetUpExecuted)
    {
        _settings = settings;
        _onSetUpExecuted = onSetUpExecuted;
        _container = settings.HostingType switch
        {
            DbHostingType.ManagedDb => new ManagedMongoDbContainer(settings.ManagedDb!),
            DbHostingType.ExternalDb => new ExternalMongoDbContainer(settings.ExternalDb!),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.HostingType), settings.HostingType, "Unknown DB hosting type")
        };
    }

    public string ConnectionString => _container.ConnectionString;

    public async Task SetUpAsync()
    {
        await _container.StartAsync();

        var mongoClient = new MongoClient(_container.ConnectionString);
        var database = mongoClient.GetDatabase(_settings.Database);
        _dbInitializer = new GymDiaryTestDbInitializer(
            database,
            new MongoMigrator(database, NullLogger<MongoMigrator>.Instance)
        );
        await _dbInitializer.Initialize();

        _onSetUpExecuted?.Invoke(this);
    }

    public Task Reset() => _dbInitializer?.Cleanup() ?? Task.CompletedTask;

    public Task TearDownAsync() => _container.DisposeAsync().AsTask();

    public void Dispose() => _container.DisposeAsync().GetAwaiter().GetResult();
}
