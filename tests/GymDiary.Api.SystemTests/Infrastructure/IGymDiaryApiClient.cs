using GymDiary.Api.SystemTests.Infrastructure.GymDiaryApiModels;

using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure;

public interface IGymDiaryApiClient
{
    [Get("/ping")]
    Task<IApiResponse> Ping();

    [Post("/v1/sportsmen/{sportsmanId}/exerciseCategories")]
    Task<IApiResponse> CreateExerciseCategory(string sportsmanId, CreateExerciseCategoryRequest request);
}
