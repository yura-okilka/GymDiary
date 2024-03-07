using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;

using Refit;

using SystemTests.Assertions;

namespace GymDiary.Api.SystemTests.Contexts;

public class PingContext(IGymDiaryApp gymDiaryApp, IGymDiaryDb gymDiaryDb) : GymDiaryContextBase(gymDiaryApp, gymDiaryDb)
{
    private IApiResponse _pingResponse = null!;

    public async Task Call_ping()
    {
        _pingResponse = await Api.Ping();
    }

    public Task Ping_response_should_have_success()
    {
        return _pingResponse.ShouldHaveSuccess();
    }
}
