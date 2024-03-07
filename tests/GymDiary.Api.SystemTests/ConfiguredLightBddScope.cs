using GymDiary.Api.SystemTests;
using GymDiary.Api.SystemTests.TestDb;
using GymDiary.Api.SystemTests.TestServer;

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

        // The order is important because server depends on DB connection string generated on DB setup.
        configuration
            .ExecutionExtensionsConfiguration()
            .RegisterGlobalSetUp<GymDiaryTestDb>()
            .RegisterGlobalSetUp<GymDiaryTestServer>();
    }

    private static void ConfigureContainer(IDefaultContainerConfigurator cfg)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        var testConfiguration = new ConfigurationBuilder()
            .AddJsonFile("test.settings.json")
            .AddJsonFile($"test.settings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var testDbSettings = testConfiguration.GetSection("MongoDb").Get<GymDiaryTestDbSettings>() ??
                             throw new Exception("Failed to parse MongoDB settings");

        cfg.RegisterType<GymDiaryTestDbSettings>(InstanceScope.Single, _ => testDbSettings);
        cfg.RegisterType<GymDiaryTestDb>(
            InstanceScope.Single,
            d => new GymDiaryTestDb(
                d.Resolve<GymDiaryTestDbSettings>(),
                // ConnectionString is generated on DB container start.
                onSetUpExecuted: db => d.Resolve<GymDiaryTestServerSettings>().TestDbConnectionString = db.ConnectionString
            ),
            o => o.As<GymDiaryTestDb>().As<IGymDiaryDb>()
        );
        cfg.RegisterType<GymDiaryTestServerSettings>(
            InstanceScope.Single,
            _ => new GymDiaryTestServerSettings { TestDbName = testDbSettings.Database }
        );
        cfg.RegisterType<GymDiaryTestServer>(InstanceScope.Single, o => o.As<GymDiaryTestServer>().As<IGymDiaryApp>());
    }
}
