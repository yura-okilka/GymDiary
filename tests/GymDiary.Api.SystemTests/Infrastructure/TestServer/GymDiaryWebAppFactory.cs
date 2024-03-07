using GymDiary.Api.SystemTests.Infrastructure.TestServer.Fakes;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer;

public record GymDiaryAppSettings(string TestDbConnectionString, string TestDbName);

public class GymDiaryWebAppFactory(GymDiaryAppSettings settings, GymDiaryAppFakes fakes)
    : WebApplicationFactory<Program.TestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(
                b => b.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["MongoDb:ConnectionString"] = settings.TestDbConnectionString,
                        ["MongoDb:Database"] = settings.TestDbName
                    }
                )
            )
            .ConfigureLogging(b => b.ClearProviders())
            .ConfigureServices(
                services => services.Replace(ServiceDescriptor.Singleton(fakes.Clock))
            );
    }
}
