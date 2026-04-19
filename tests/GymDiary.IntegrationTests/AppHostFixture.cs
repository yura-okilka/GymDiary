using Aspire.Hosting;
using Aspire.Hosting.Testing;

using GymDiary.ApiClient;
using GymDiary.ApiClient.Generated;
using GymDiary.AppHost;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

using Projects;

namespace GymDiary.IntegrationTests;

/// <summary>
///     Boots the GymDiary AppHost once per xUnit collection and keeps it alive for every test in that collection.
///     Starting the AppHost spins up the MongoDB container and the API process, which is expensive — sharing the
///     instance across tests is the key efficiency lever recommended by Aspire's testing guidance.
/// </summary>
public sealed class AppHostFixture : IAsyncLifetime
{
    private static readonly TimeSpan ResourceReadyTimeout = TimeSpan.FromSeconds(30);

    private DistributedApplication? _app;
    private ServiceProvider? _appClientServices;

    public DistributedApplication App => _app ?? throw new InvalidOperationException("AppHost has not been initialized yet.");

    private IServiceProvider ApiClientServices =>
        _appClientServices ?? throw new InvalidOperationException("AppHost has not been initialized yet.");

    public GymDiaryApiClient GetApiClient() => ApiClientServices.GetRequiredService<GymDiaryApiClient>();

    public async Task InitializeAsync()
    {
        _app = await StartAppHostAsync();
        _appClientServices = BuildApiClientServices(_app.CreateHttpClient(ResourceNames.Api).BaseAddress!);
    }

    public async Task DisposeAsync()
    {
        if (_appClientServices is not null)
        {
            await _appClientServices.DisposeAsync();
        }

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    private static async Task<DistributedApplication> StartAppHostAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<GymDiary_AppHost>();
        TrustDevCertificate(builder.Services);

        var app = await builder.BuildAsync();
        await app.StartAsync();
        await WaitForApiHealthyAsync(app);
        return app;
    }

    private static async Task WaitForApiHealthyAsync(DistributedApplication app)
    {
        using var cts = new CancellationTokenSource(ResourceReadyTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Api, cts.Token);
    }

    /// <summary>
    ///     Builds the Kiota client the same way production does — via <see cref="ApiClientServiceExtensions.AddGymDiaryApiClient" />
    ///     — so tests exercise Kiota's default handler pipeline (retry, redirect, URL replacement, parameter-name decoding,
    ///     user-agent, headers inspection). Just constructing <see cref="GymDiaryApiClientFactory" /> with a bare
    ///     <see cref="HttpClient" /> would skip every one of those <see cref="DelegatingHandler" />s.
    /// </summary>
    private static ServiceProvider BuildApiClientServices(Uri apiBaseAddress)
    {
        var services = new ServiceCollection();
        services.AddGymDiaryApiClient(http => http.BaseAddress = apiBaseAddress);
        TrustDevCertificate(services);
        return services.BuildServiceProvider();
    }

    /// <summary>
    ///     The API exposes HTTPS endpoints signed by the ASP.NET Core dev certificate, which isn't trusted inside a
    ///     test host. Override the primary handler of every named HttpClient the container creates.
    ///     <see cref="OptionsServiceCollectionExtensions.PostConfigureAll{TOptions}" /> runs after every per-client
    ///     configure step, so the override wins even on clients that install their own primary handler later in the
    ///     configure phase — notably the URL health-check client Aspire's <c>WithHttpHealthCheck</c> creates, which
    ///     <see cref="HttpClientFactoryServiceCollectionExtensions.ConfigureHttpClientDefaults" /> did not reach in practice.
    /// </summary>
    private static void TrustDevCertificate(IServiceCollection services) =>
        services.PostConfigureAll<HttpClientFactoryOptions>(options =>
            options.HttpMessageHandlerBuilderActions.Add(b => b.PrimaryHandler = CreateCertificateIgnoringHandler()));

    private static HttpClientHandler CreateCertificateIgnoringHandler() =>
        new() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
}
