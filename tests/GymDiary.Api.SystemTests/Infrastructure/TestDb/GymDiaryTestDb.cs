using GymDiary.DbMigrations;

using LightBDD.Core.Execution;

using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Driver;

using Testcontainers.MongoDb;

namespace GymDiary.Api.SystemTests.Infrastructure.TestDb;

public record GymDiaryTestDbSettings(string Database, string Image);

// TODO: Own Managed DB external DB?
public class GymDiaryTestDb : IDisposable, IGlobalResourceSetUp
{
    private readonly GymDiaryTestDbSettings _settings;
    private readonly MongoDbContainer _container;
    private readonly Action<GymDiaryTestDb>? _onSetUpExecuted;
    private GymDiaryTestDbInitializer? _dbInitializer;

    public GymDiaryTestDb(GymDiaryTestDbSettings settings, Action<GymDiaryTestDb>? onSetUpExecuted)
    {
        _container = new MongoDbBuilder().WithImage(settings.Image).Build();
        _settings = settings;
        _onSetUpExecuted = onSetUpExecuted;
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task SetUpAsync()
    {
        await _container.StartAsync();

        var mongoClient = new MongoClient(_container.GetConnectionString());
        var database = mongoClient.GetDatabase(_settings.Database);
        _dbInitializer = new GymDiaryTestDbInitializer(
            database,
            new MongoMigrator(database, NullLogger<MongoMigrator>.Instance)
        );
        await _dbInitializer.Initialize();

        _onSetUpExecuted?.Invoke(this);
    }

    public Task ResetAsync() => _dbInitializer?.Cleanup() ?? Task.CompletedTask;

    public Task TearDownAsync() => _container.DisposeAsync().AsTask();

    public void Dispose() => _container.DisposeAsync().GetAwaiter().GetResult();
}
