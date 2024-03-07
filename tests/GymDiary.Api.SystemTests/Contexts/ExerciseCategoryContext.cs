using System.Net;

using Bogus;

using GymDiary.Api.SystemTests.Infrastructure.TestDb;
using GymDiary.Api.SystemTests.Infrastructure.TestServer;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiClients.Models;

using MongoDB.Bson;

using Refit;

using SystemTests.Assertions;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(IGymDiaryApp gymDiaryApp, IGymDiaryDb gymDiaryDb)
    : GymDiaryContextBase(gymDiaryApp, gymDiaryDb)
{
    private IApiResponse _createExerciseCategoryResponse = null!;
    public IApiResponse<CreateSportsman.Response> CreateSportsmanResponse { get; private set; } = null!;
    public string SportsmanId { get; private set; } = null!;

    public Task Create_exercise_category_for_unknown_sportsman()
    {
        return Create_exercise_category(ObjectId.GenerateNewId().ToString(), "Cardio");
    }

    public async Task Create_exercise_category(string sportsmanId, string name)
    {
        SportsmanId = sportsmanId;
        _createExerciseCategoryResponse = await Api.CreateExerciseCategory(
            sportsmanId,
            new CreateExerciseCategory.Request(name)
        );
    }

    public async Task Create_sportsman()
    {
        var requestFaker = new Faker<CreateSportsman.Request>()
            .RuleFor(r => r.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, f => f.Date.Past(yearsToGoBack: 20, DateTime.UtcNow));
        //.RuleFor(r => r.Gender, f => f.PickRandom<CreateSportsman.GenderType>()); // "Male", "Female", "Other"

        var request = requestFaker.Generate();

        CreateSportsmanResponse = await Api.CreateSportsman(request);
    }

    public Task Create_sportsman_response_should_have_success(HttpStatusCode statusCode)
    {
        return CreateSportsmanResponse.ShouldHaveSuccess(statusCode);
    }

    public Task Create_exercise_category_response_should_have_error<TError>(HttpStatusCode statusCode, TError error)
    {
        return _createExerciseCategoryResponse.ShouldHaveError(statusCode, error);
    }

    public Task Create_exercise_category_response_should_have_success(HttpStatusCode statusCode)
    {
        // TODO: use Expectation Expressions?
        return _createExerciseCategoryResponse.ShouldHaveSuccess(statusCode);
    }
}
