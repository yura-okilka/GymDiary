using GymDiary.Api.SystemTests.Infrastructure.TestServer;

using Refit;

using SystemTests.Assertions;

namespace GymDiary.Api.SystemTests.Contexts;

public class PingContext(GymDiaryTestServer gymDiaryApi)
{
    private IApiResponse _pingResponse = null!;

    public async Task Call_ping()
    {
        _pingResponse = await gymDiaryApi.Client.Ping();
    }

    public Task Ping_response_should_be_successful()
    {
        _pingResponse.ShouldBeSuccessful();
        return Task.CompletedTask;
    }
}
