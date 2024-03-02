using FluentAssertions;

using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.GymDiaryApiModels;

using MongoDB.Bson;

using Refit;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(GymDiaryTestServer gymDiaryApi, GymDiaryTestDb gymDiaryDb)
    : GymDiaryContextBase(gymDiaryApi, gymDiaryDb)
{
    private IApiResponse? _createExerciseCategoryResponse;

    public async Task Create_exercise_category()
    {
        _createExerciseCategoryResponse = await GymDiaryApi.Client.CreateExerciseCategory(
            sportsmanId: ObjectId.GenerateNewId().ToString(),
            request: new CreateExerciseCategoryRequest("ExerciseCategory1")
        );
    }

    public Task Create_exercise_category_response_should_be_successful()
    {
        // TODO: use Expectation Expressions?
        _createExerciseCategoryResponse!.IsSuccessStatusCode.Should().BeTrue();
        return Task.CompletedTask;
    }
}
