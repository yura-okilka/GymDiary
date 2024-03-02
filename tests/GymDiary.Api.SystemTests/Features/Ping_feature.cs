using GymDiary.Api.SystemTests.Contexts;

using SystemTests.BddRunner;

namespace GymDiary.Api.SystemTests.Features;

public class Ping_feature : FeatureFixture
{
    [Scenario]
    public async Task Call_ping()
    {
        await Runner
            .WithContext<PingContext>()
            .RunScenarioWithEnvCleanup(
                when => when.Call_ping(),
                then => then.Ping_response_should_have_success()
            );
    }
}
