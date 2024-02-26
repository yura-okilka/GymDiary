using Microsoft.Extensions.Time.Testing;

namespace SystemTests.Context;

public interface ITestContextFakeTime
{
    FakeTimeProvider Time { get; }
}

/// <summary>
///     Extension methods for managing time in tests.
/// </summary>
public static class TestContextFakeTimeExtensions
{
    public static Task Set_UTC_now(this ITestContextFakeTime context)
    {
        context.Time.SetUtcNow(DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public static Task Set_UTC_now(this ITestContextFakeTime context, DateTime value)
    {
        context.Time.SetUtcNow(value);
        return Task.CompletedTask;
    }

    public static Task Advance_time(this ITestContextFakeTime context, TimeSpan delta)
    {
        context.Time.Advance(delta);
        return Task.CompletedTask;
    }
}
