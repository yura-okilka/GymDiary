namespace GymDiary.Api.SystemTests.TestApp.Fakes;

public class GymDiaryAppFakes
{
    public FakeClock Clock { get; } = new();

    public void Reset() => Clock.Reset();
}
