namespace GymDiary.Api.SystemTests.TestDb;

public interface IDbContainer : IAsyncDisposable
{
    string ConnectionString { get; }
    Task StartAsync();
}
