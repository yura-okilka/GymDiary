using GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients.Models;

using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients;

public interface IGymDiaryApiClient
{
    [Get("/ping")]
    Task<IApiResponse> Ping();

    [Post("/v1/sportsmen/{sportsmanId}/exerciseCategories")]
    Task<IApiResponse> CreateExerciseCategory(string sportsmanId, CreateExerciseCategory.Request request);

    [Post("/v1/sportsmen")]
    Task<IApiResponse<CreateSportsman.Response>> CreateSportsman(CreateSportsman.Request request);
}
