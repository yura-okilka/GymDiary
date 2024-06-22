using System.Net;

using GymDiary.Api.SystemTests.Contexts;
using GymDiary.Api.SystemTests.TestApp.ApiClients.Models;

using SystemTests.BddRunner;

namespace GymDiary.Api.SystemTests.Features;

public class Exercise_category_feature : FeatureFixture
{
    [Scenario]
    public async Task Create_exercise_category()
    {
        await Runner
            .WithContext<ExerciseCategoryContext>()
            .RunScenarioWithEnvCleanup(
                given => given.Create_user(),
                given => given.Create_user_response_should_have_success(HttpStatusCode.Created),
                when => when.Create_exercise_category(when.CreateUserResponse.Content!.Id, "Cardio"),
                then => then.Create_exercise_category_response_should_have_success(HttpStatusCode.Created)
            );
    }

    [Scenario]
    public async Task Create_exercise_category_for_unknown_user_should_fail()
    {
        await Runner
            .WithContext<ExerciseCategoryContext>()
            .RunScenarioWithEnvCleanup(
                when => when.Create_exercise_category_for_unknown_user(),
                then => then.Create_exercise_category_response_should_have_error(
                    HttpStatusCode.Conflict,
                    CreateExerciseCategory.OwnerNotFoundError(then.UserId)
                )
            );
    }
}
