using System.Text.Json;
using System.Text.Json.Serialization;

using LightBDD.Core.Execution;

using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure;

public record GymDiaryTestServerSettings
{
    public string? TestDbConnectionString { get; set; }
}

public class GymDiaryTestServer : IDisposable, IGlobalResourceSetUp
{
    private readonly GymDiaryWebApplicationFactory _testServer;

    public IGymDiaryApiClient Client { get; }

    public GymDiaryTestServer(GymDiaryTestServerSettings settings)
    {
        var testDbConnectionString = settings.TestDbConnectionString ??
                                     throw new ArgumentNullException(nameof(settings.TestDbConnectionString));

        _testServer = new GymDiaryWebApplicationFactory(new GymDiaryApiSettings(testDbConnectionString));
        var httpClient = _testServer.CreateDefaultClient();

        Client = RestService.For<IGymDiaryApiClient>(
            httpClient,
            new RefitSettings(
                new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        Converters = { new JsonStringEnumConverter() }
                    }
                )
            )
        );
    }

    public Task SetUpAsync() => Task.CompletedTask;

    public async Task TearDownAsync() => await _testServer.DisposeAsync();

    public void Dispose() => _testServer.Dispose();
}
