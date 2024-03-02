using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;

using SystemTests.BddRunner;
using SystemTests.Context;

namespace GymDiary.Api.SystemTests.Contexts;

public abstract class GymDiaryContextBase(GymDiaryTestServer gymDiaryApi, GymDiaryTestDb gymDiaryDb)
    : ITestContextEnvCleanup, ITestContextStorage, ITestContextFakeTime
{
    protected readonly GymDiaryTestServer GymDiaryApi = gymDiaryApi;
    protected readonly GymDiaryTestDb GymDiaryDb = gymDiaryDb;
    public Dictionary<string, object?> Storage { get; } = new();

    public async Task Clean_up_environment()
    {
        await GymDiaryDb.ResetAsync();
        await GymDiaryApi.ResetAsync();
    }

    public Task Set_UTC_now(DateTime value)
    {
        GymDiaryApi.Clock.SetUtcNow(value);
        return Task.CompletedTask;
    }

    public Task Advance_time(TimeSpan delta)
    {
        GymDiaryApi.Clock.Advance(delta);
        return Task.CompletedTask;
    }
}
