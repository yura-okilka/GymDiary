using System.Text.Json;
using System.Text.Json.Serialization;

using GymDiary.Api.SystemTests.TestApp.ApiClients;
using GymDiary.Api.SystemTests.TestApp.Fakes;

using LightBDD.Core.Execution;

using Refit;

namespace GymDiary.Api.SystemTests.TestApp;

public class GymDiaryTestApp : IDisposable, IGlobalResourceSetUp, IGymDiaryApp
{
    private readonly GymDiaryTestAppFactory _appFactory;

    public IGymDiaryApiClient Client { get; }
    public GymDiaryAppFakes Fakes { get; } = new();

    public GymDiaryTestApp(GymDiaryTestAppSettings settings)
    {
        _appFactory = new GymDiaryTestAppFactory(settings, Fakes);
        var httpClient = _appFactory.CreateDefaultClient();

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

    public async Task TearDownAsync() => await _appFactory.DisposeAsync();

    public void Dispose() => _appFactory.Dispose();
}
