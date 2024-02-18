using LightBDD.Core.Execution;

using Testcontainers.MongoDb;

namespace GymDiary.Api.SystemTests.Infrastructure;

public record GymDiaryTestDbSettings(string Image);

// TODO: Own Managed DB external DB?
public class GymDiaryTestDb : IDisposable, IGlobalResourceSetUp
{
    private readonly MongoDbContainer _container;
    private readonly Action<GymDiaryTestDb>? _onSetUpExecuted;

    public GymDiaryTestDb(GymDiaryTestDbSettings settings, Action<GymDiaryTestDb>? onSetUpExecuted)
    {
        _container = new MongoDbBuilder().WithImage(settings.Image).Build();
        _onSetUpExecuted = onSetUpExecuted;
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task SetUpAsync()
    {
        await _container.StartAsync();
        _onSetUpExecuted?.Invoke(this);
    }

    public Task TearDownAsync() => _container.DisposeAsync().AsTask();

    public void Dispose() => _container.DisposeAsync().GetAwaiter().GetResult();
}
