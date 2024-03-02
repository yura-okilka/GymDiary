namespace SystemTests.Context;

public interface ITestContextFakeTime
{
    Task Set_UTC_now(DateTime value);
    Task Advance_time(TimeSpan delta);
}
