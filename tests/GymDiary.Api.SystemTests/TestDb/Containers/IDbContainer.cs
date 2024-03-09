namespace GymDiary.Api.SystemTests.TestDb.Containers;

public interface IDbContainer : IAsyncDisposable
{
    string ConnectionString { get; }
    Task StartAsync();
}
