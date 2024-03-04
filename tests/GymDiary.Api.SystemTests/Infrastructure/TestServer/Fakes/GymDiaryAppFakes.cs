namespace GymDiary.Api.SystemTests.Infrastructure.TestServer.Fakes;

public class GymDiaryAppFakes
{
    public FakeClock Clock { get; } = new();

    public void Reset() => Clock.Reset();
}
