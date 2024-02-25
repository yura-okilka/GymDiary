using GymDiary.Api.SystemTests.Infrastructure.TestServer.GymDiaryApiModels;

using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer;

public interface IGymDiaryApiClient
{
    [Get("/ping")]
    Task<IApiResponse> Ping();

    [Post("/v1/sportsmen/{sportsmanId}/exerciseCategories")]
    Task<IApiResponse> CreateExerciseCategory(string sportsmanId, CreateExerciseCategoryRequest request);
}
