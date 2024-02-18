using System.Text.Json;
using System.Text.Json.Serialization;

using LightBDD.Core.Execution;

using Microsoft.AspNetCore.Mvc.Testing;

using Refit;

namespace GymDiary.Api.SystemTests.Infrastructure;

public class GymDiaryApiTestServer : IDisposable, IGlobalResourceSetUp
{
    private readonly WebApplicationFactory<Program.TestEntryPoint> _testServer;

    public IGymDiaryApiClient Client { get; }

    public GymDiaryApiTestServer()
    {
        _testServer = new WebApplicationFactory<Program.TestEntryPoint>();
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

    public async Task TearDownAsync() => await _testServer.DisposeAsync(); // TODO: check calls

    public void Dispose() => _testServer.Dispose();
}
