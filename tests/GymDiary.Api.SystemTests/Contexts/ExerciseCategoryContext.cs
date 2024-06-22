using System.Net;

using Bogus;

using GymDiary.Api.SystemTests.TestApp;
using GymDiary.Api.SystemTests.TestApp.ApiClients.Models;
using GymDiary.Api.SystemTests.TestDb;

using MongoDB.Bson;

using Refit;

using SystemTests.Assertions;

namespace GymDiary.Api.SystemTests.Contexts;

public class ExerciseCategoryContext(IGymDiaryApp gymDiaryApp, IGymDiaryDb gymDiaryDb)
    : GymDiaryContextBase(gymDiaryApp, gymDiaryDb)
{
    private IApiResponse _createExerciseCategoryResponse = null!;
    public IApiResponse<CreateUser.Response> CreateUserResponse { get; private set; } = null!;
    public string UserId { get; private set; } = null!;

    public Task Create_exercise_category_for_unknown_user()
    {
        return Create_exercise_category(ObjectId.GenerateNewId().ToString(), "Cardio");
    }

    public async Task Create_exercise_category(string userId, string name)
    {
        UserId = userId;
        _createExerciseCategoryResponse = await Api.CreateExerciseCategory(
            userId,
            new CreateExerciseCategory.Request(name)
        );
    }

    public async Task Create_user()
    {
        var requestFaker = new Faker<CreateUser.Request>()
            .RuleFor(r => r.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, f => f.Date.Past(yearsToGoBack: 20, DateTime.UtcNow));
        //.RuleFor(r => r.Gender, f => f.PickRandom<CreateUser.GenderType>()); // "Male", "Female", "Other"

        var request = requestFaker.Generate();

        CreateUserResponse = await Api.CreateUser(request);
    }

    public Task Create_user_response_should_have_success(HttpStatusCode statusCode)
    {
        return CreateUserResponse.ShouldHaveSuccess(statusCode);
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
