using Testcontainers.MongoDb;

namespace GymDiary.Api.SystemTests.Infrastructure.TestDb;

public record ManagedMongoDbContainerSettings(string Image);

public class ManagedMongoDbContainer : IDbContainer
{
    private readonly MongoDbContainer _container;

    public ManagedMongoDbContainer(ManagedMongoDbContainerSettings settings)
    {
        _container = new MongoDbBuilder().WithImage(settings.Image).Build();
    }

    public string ConnectionString => _container.GetConnectionString();

    public Task StartAsync() => _container.StartAsync();

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
