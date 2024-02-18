using GymDiary.Api.SystemTests;
using GymDiary.Api.SystemTests.Infrastructure;

using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;

using Microsoft.Extensions.Configuration;

/*
 * This is a way to enable LightBDD - XUnit integration.
 * It is required to do it in all assemblies with LightBDD scenarios.
 * It is possible to either use [assembly:LightBddScope] directly to use LightBDD with default configuration,
 * or customize it in a way that is shown below.
 */
[assembly: ConfiguredLightBddScope]

namespace GymDiary.Api.SystemTests;

/// <summary>
///     This class extends LightBddScopeAttribute and allows to customize the default configuration of LightBDD.
///     It is also possible here to override OnSetUp() and OnTearDown() methods to execute code that has to be run once,
///     before or after all tests.
/// </summary>
public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
{
    /// <summary>
    ///     This method allows to customize LightBDD behavior.
    ///     The code below configures LightBDD to produce also a plain text report after all tests are done.
    ///     More information on what can be customized can be found on wiki:
    ///     https://github.com/LightBDD/LightBDD/wiki/LightBDD-Configuration#configurable-lightbdd-features
    /// </summary>
    protected override void OnConfigure(LightBddConfiguration configuration)
    {
        configuration
            .DependencyContainerConfiguration()
            .UseDefault(ConfigureContainer);

        configuration
            .ExecutionExtensionsConfiguration()
            .RegisterGlobalSetUp<GymDiaryApiTestServer>();
    }

    private static void ConfigureContainer(IDefaultContainerConfigurator cfg)
    {
        var testConfiguration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .AddEnvironmentVariables()
            .Build();

        cfg.RegisterType<GymDiaryApiTestServer>(InstanceScope.Single);
    }
}
