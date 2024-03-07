using System.Text.Json;
using System.Text.Json.Serialization;

using GymDiary.Api.SystemTests.TestServer.ApiClients;
using GymDiary.Api.SystemTests.TestServer.Fakes;

using LightBDD.Core.Execution;

using Refit;

namespace GymDiary.Api.SystemTests.TestServer;

public record GymDiaryTestServerSettings
{
    public required string DbName { get; init; }
    public string? DbConnectionString { get; set; }
}

public class GymDiaryTestServer : IDisposable, IGlobalResourceSetUp, IGymDiaryApp
{
    private readonly GymDiaryTestAppFactory _testServer;

    public IGymDiaryApiClient Client { get; }
    public GymDiaryAppFakes Fakes { get; } = new();

    public GymDiaryTestServer(GymDiaryTestServerSettings settings)
    {
        var dbConnectionString = settings.DbConnectionString ?? throw new ArgumentNullException(nameof(settings.DbConnectionString));

        _testServer = new GymDiaryTestAppFactory(new GymDiaryTestAppSettings(dbConnectionString, settings.DbName), Fakes);
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

    public void Reset() => Fakes.Reset();

    public async Task TearDownAsync() => await _testServer.DisposeAsync();

    public void Dispose() => _testServer.Dispose();
}
