using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GymDiary.Api.SystemTests.Infrastructure;

public class GymDiaryWebApplicationFactory(string TestDbConnectionString) : WebApplicationFactory<Program.TestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(
                b => b.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["MongoDb:ConnectionString"] = TestDbConnectionString
                    }
                )
            )
            .ConfigureServices(
                services =>
                {
                    //var mongoClientDescriptor = services.Single(d => d.ServiceType == typeof(IMongoClient));

                    //services.Remove(mongoClientDescriptor);

                    //var mongoClient = new MongoClient(connectionString);
                }
            );
    }
}
