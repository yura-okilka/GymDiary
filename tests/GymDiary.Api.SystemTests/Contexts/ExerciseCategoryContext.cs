using System.Net;

using FluentAssertions;

using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.GymDiaryApiModels;

using MongoDB.Bson;

using Refit;

using SystemTests.Assertions;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(IGymDiaryApp gymDiaryApp, IGymDiaryDb gymDiaryDb)
    : GymDiaryContextBase(gymDiaryApp, gymDiaryDb)
{
    private string? _sportsmanId;
    private IApiResponse? _createExerciseCategoryResponse;

    public string SportsmanId => _sportsmanId!;

    public Task Create_exercise_category_for_unknown_sportsman()
    {
        return Create_exercise_category(ObjectId.GenerateNewId().ToString(), "Cardio");
    }

    public async Task Create_exercise_category(string sportsmanId, string name)
    {
        _sportsmanId = sportsmanId;
        _createExerciseCategoryResponse = await Api.CreateExerciseCategory(
            sportsmanId,
            new CreateExerciseCategory.Request(name)
        );
    }

    public Task Create_exercise_category_response_should_have_error<TError>(HttpStatusCode statusCode, TError error)
    {
        return _createExerciseCategoryResponse!.ShouldHaveError(statusCode, error);
    }

    public Task Create_exercise_category_response_should_have(HttpStatusCode statusCode)
    {
        // TODO: use Expectation Expressions?
        _createExerciseCategoryResponse!.StatusCode.Should().Be(statusCode);
        return Task.CompletedTask;
    }
}
