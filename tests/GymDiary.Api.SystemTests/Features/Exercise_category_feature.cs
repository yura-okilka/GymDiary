using GymDiary.Api.SystemTests.Contexts;

namespace GymDiary.Api.SystemTests.Features;

public class Exercise_category_feature : FeatureFixture
{
    [Scenario]
    public async Task Create_exercise_category()
    {
        await Runner
            .WithContext<ExerciseCategoryContext>()
            .RunScenarioAsync(
                given => given.Ping(),
                then => then.Ping_response_should_be_successful()
            );
    }
}
