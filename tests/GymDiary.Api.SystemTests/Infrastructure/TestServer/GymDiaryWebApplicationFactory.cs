using GymDiary.Core.Time;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer;

public record GymDiaryApiSettings(string TestDbConnectionString, IClock Clock);

public class GymDiaryWebApplicationFactory(GymDiaryApiSettings settings) : WebApplicationFactory<Program.TestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(
                b => b.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["MongoDb:ConnectionString"] = settings.TestDbConnectionString
                    }
                )
            )
            .ConfigureServices(
                services => services.Replace(ServiceDescriptor.Singleton(settings.Clock))
            );
    }
}
