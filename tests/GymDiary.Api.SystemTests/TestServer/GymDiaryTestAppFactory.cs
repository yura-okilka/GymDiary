using GymDiary.Api.SystemTests.TestServer.Fakes;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace GymDiary.Api.SystemTests.TestServer;

public record GymDiaryTestAppSettings(string DbConnectionString, string DbName);

public class GymDiaryTestAppFactory(GymDiaryTestAppSettings settings, GymDiaryAppFakes fakes)
    : WebApplicationFactory<Program.TestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(
                b => b
                    .SetBasePath(Directory.GetCurrentDirectory()) // switch from app directory to tests one
                    .AddJsonFile("gymdiary.appsettings.json")
                    .AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["MongoDb:ConnectionString"] = settings.DbConnectionString,
                            ["MongoDb:Database"] = settings.DbName
                        }
                    )
            )
            .ConfigureLogging(b => b.ClearProviders())
            .ConfigureServices(
                services => services.Replace(ServiceDescriptor.Singleton(fakes.Clock))
            );
        // TODO: check env
    }
}
