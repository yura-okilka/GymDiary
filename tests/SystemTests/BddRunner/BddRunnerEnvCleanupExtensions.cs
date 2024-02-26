using System.Linq.Expressions;

using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace SystemTests.BddRunner;

public interface ITestContextEnvCleanup
{
    Task Clean_up_environment();
}

public static class BddRunnerEnvCleanupExtensions
{
    /// <summary>
    ///     Extension method for running 'Clean up environment' step before main <paramref name="steps" />.
    /// </summary>
    public static Task RunScenarioWithEnvCleanup<TContext>(
        this IBddRunner<TContext> runner,
        params Expression<Func<TContext, Task>>[] steps
    ) where TContext : ITestContextEnvCleanup
    {
        Expression<Func<TContext, Task>> cleanUp = setup => setup.Clean_up_environment();

        return runner.RunScenarioAsync([cleanUp, .. steps]);
    }
}
