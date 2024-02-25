using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GymDiary.Api.SystemTests.Infrastructure.TestServer;

public record GymDiaryApiSettings(string TestDbConnectionString);

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
            );
    }
}
