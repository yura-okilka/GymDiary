namespace GymDiary.Api.SystemTests.TestDb;

public record ExternalMongoDbContainerSettings(string ConnectionString);

public class ExternalMongoDbContainer(ExternalMongoDbContainerSettings settings) : IDbContainer
{
    public string ConnectionString => settings.ConnectionString;

    public Task StartAsync() => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
