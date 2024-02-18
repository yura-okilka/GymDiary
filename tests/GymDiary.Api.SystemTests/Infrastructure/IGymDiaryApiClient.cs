using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure;

public interface IGymDiaryApiClient
{
    [Get("/ping")]
    Task<IApiResponse> Ping();
}
