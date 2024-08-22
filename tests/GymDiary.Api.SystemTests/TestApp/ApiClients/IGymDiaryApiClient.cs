using GymDiary.Api.SystemTests.TestApp.ApiClients.Models;

using Refit;

namespace GymDiary.Api.SystemTests.TestApp.ApiClients;

public interface IGymDiaryApiClient
{
    [Get("/ping")]
    Task<IApiResponse> Ping();

    [Post("/v1/users/{userId}/exercise-categories")]
    Task<IApiResponse> CreateExerciseCategory(string userId, CreateExerciseCategory.Request request);

    [Post("/v1/users")]
    Task<IApiResponse<CreateUser.Response>> CreateUser(CreateUser.Request request);
}
