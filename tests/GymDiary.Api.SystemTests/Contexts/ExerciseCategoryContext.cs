using FluentAssertions;

using GymDiary.Api.SystemTests.Infrastructure;
using GymDiary.Api.SystemTests.Infrastructure.GymDiaryApiModels;

using MongoDB.Bson;

using Refit;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(GymDiaryTestServer gymDiaryApi)
{
    private IApiResponse? _createExerciseCategoryResponse;

    public async Task Create_exercise_category()
    {
        _createExerciseCategoryResponse = await gymDiaryApi.Client.CreateExerciseCategory(
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
