using GymDiary.Api.SystemTests.Contexts;

namespace GymDiary.Api.SystemTests.Features;

public class Ping_feature : FeatureFixture
{
    [Scenario]
    public async Task Call_ping()
    {
        await Runner
            .WithContext<PingContext>()
            .RunScenarioAsync(
                when => when.Call_ping(),
                then => then.Ping_response_should_be_successful()
            );
    }
}
