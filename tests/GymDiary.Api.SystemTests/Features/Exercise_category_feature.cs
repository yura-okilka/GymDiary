using System.Net;

using GymDiary.Api.SystemTests.Contexts;
using GymDiary.Api.SystemTests.Infrastructure.TestServer.ApiModels;

using SystemTests.BddRunner;

namespace GymDiary.Api.SystemTests.Features;

public class Exercise_category_feature : FeatureFixture
{
    [Scenario]
    public async Task Create_exercise_category_for_unknown_sportsman_should_fail()
    {
        await Runner
            .WithContext<ExerciseCategoryContext>()
            .RunScenarioWithEnvCleanup(
                when => when.Create_exercise_category_for_unknown_sportsman(),
                then => then.Create_exercise_category_response_should_have_error(
                    HttpStatusCode.Conflict,
                    CreateExerciseCategory.OwnerNotFoundError(then.SportsmanId)
                )
            );
    }
}
