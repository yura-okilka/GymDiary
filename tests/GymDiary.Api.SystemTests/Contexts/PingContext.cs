using FluentAssertions;

using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;

using Refit;

namespace GymDiary.Api.SystemTests.Contexts;

public class PingContext(IGymDiaryApp gymDiaryApp, GymDiaryTestDb gymDiaryDb) : GymDiaryContextBase(gymDiaryApp, gymDiaryDb)
{
    private IApiResponse _pingResponse = null!;

    public async Task Call_ping()
    {
        _pingResponse = await Api.Ping();
    }

    public Task Ping_response_should_have_success()
    {
        _pingResponse.IsSuccessStatusCode.Should().BeTrue();
        return Task.CompletedTask;
    }
}
