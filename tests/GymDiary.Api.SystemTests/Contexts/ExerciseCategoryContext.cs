using FluentAssertions;

using GymDiary.Api.SystemTests.Infrastructure;

using Refit;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(GymDiaryTestServer gymDiaryApi)
{
    private IApiResponse? _pingResponse;

    public async Task Ping()
    {
        _pingResponse = await gymDiaryApi.Client.Ping();
    }

    public Task Ping_response_should_be_successful()
    {
        _pingResponse!.IsSuccessStatusCode.Should().BeTrue();
        return Task.CompletedTask;
    }
}
