using GymDiary.Api.SystemTests.TestApp.Fakes;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace GymDiary.Api.SystemTests.TestApp;

public record GymDiaryTestAppSettings
{
    public required string DbName { get; init; }
    public string? DbConnectionString { get; set; }
}

public class GymDiaryTestAppFactory(GymDiaryTestAppSettings settings, GymDiaryAppFakes fakes)
    : WebApplicationFactory<Program.TestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Production")
            .ConfigureAppConfiguration(
                b => b
                    .SetBasePath(Directory.GetCurrentDirectory()) // Switch from app directory to tests one.
                    .AddJsonFile("gymdiary.appsettings.json")
                    .AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:MongoDb"] = settings.DbConnectionString ??
                                                            throw new ArgumentNullException(nameof(settings.DbConnectionString)),
                            ["MongoDb:Database"] = settings.DbName
                        }
                    )
            )
            .ConfigureLogging(b => b.ClearProviders())
            .ConfigureServices(
                services => services.Replace(ServiceDescriptor.Singleton(fakes.Clock))
            );
    }
}
