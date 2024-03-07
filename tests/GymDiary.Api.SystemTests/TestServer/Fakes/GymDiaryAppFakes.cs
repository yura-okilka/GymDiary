namespace GymDiary.Api.SystemTests.TestServer.Fakes;

public class GymDiaryAppFakes
{
    public FakeClock Clock { get; } = new();

    public void Reset() => Clock.Reset();
}
