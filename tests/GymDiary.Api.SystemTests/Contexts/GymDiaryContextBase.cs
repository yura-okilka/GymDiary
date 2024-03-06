using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients;

using SystemTests.BddRunner;
using SystemTests.Context;

namespace GymDiary.Api.SystemTests.Contexts;

public abstract class GymDiaryContextBase(IGymDiaryApp gymDiaryApp, IGymDiaryDb gymDiaryDb)
    : ITestContextEnvCleanup, ITestContextStorage, ITestContextFakeTime
{
    protected readonly IGymDiaryApp GymDiaryApp = gymDiaryApp;
    protected readonly IGymDiaryDb GymDiaryDb = gymDiaryDb;
    protected IGymDiaryApiClient Api => GymDiaryApp.Client;
    public Dictionary<string, object?> Storage { get; } = new();

    public async Task Clean_up_environment()
    {
        await GymDiaryDb.Reset();
        GymDiaryApp.Reset();
    }

    public Task Set_UTC_now(DateTime value)
    {
        GymDiaryApp.Fakes.Clock.SetUtcNow(value);
        return Task.CompletedTask;
    }

    public Task Advance_time(TimeSpan delta)
    {
        GymDiaryApp.Fakes.Clock.Advance(delta);
        return Task.CompletedTask;
    }
}
