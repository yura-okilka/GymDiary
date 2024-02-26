using Microsoft.Extensions.Time.Testing;

using SystemTests.BddRunner;
using SystemTests.Context;

namespace SystemTests;

/// <summary>
///     Default context for tests that need environment cleanup, storage for intermediate step results and time management.
/// </summary>
public abstract class DefaultTestContext : ITestContextEnvCleanup, ITestContextStorage, ITestContextFakeTime
{
    public Dictionary<string, object?> Storage { get; } = new();
    public abstract FakeTimeProvider Time { get; }
    public abstract Task Clean_up_environment();
}
