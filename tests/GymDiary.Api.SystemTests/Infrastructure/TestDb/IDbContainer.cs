namespace GymDiary.Api.SystemTests.Infrastructure.TestDb;

public interface IDbContainer : IAsyncDisposable
{
    string ConnectionString { get; }
    Task StartAsync();
}
