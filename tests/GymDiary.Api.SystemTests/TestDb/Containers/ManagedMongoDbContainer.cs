using Testcontainers.MongoDb;

namespace GymDiary.Api.SystemTests.TestDb.Containers;

public record ManagedMongoDbContainerSettings(string Image);

public class ManagedMongoDbContainer : IDbContainer
{
    private readonly MongoDbContainer _container;

    public ManagedMongoDbContainer(ManagedMongoDbContainerSettings settings)
    {
        _container = new MongoDbBuilder(settings.Image).Build();
    }

    public string ConnectionString => _container.GetConnectionString();

    public Task StartAsync() => _container.StartAsync();

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
