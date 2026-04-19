using Microsoft.Extensions.DependencyInjection;

namespace GymDiary.ApiClient;

public static class ApiClientServiceExtensions
{
    /// <summary>
    ///     Registers the <see cref="Generated.GymDiaryApiClient" /> and its dependencies in the service collection.
    ///     Source: <see href="https://learn.microsoft.com/en-us/openapi/kiota/tutorials/dotnet-dependency-injection#register-the-api-client" />.
    /// </summary>
    public static IServiceCollection AddGymDiaryApiClient(this IServiceCollection services, Action<HttpClient>? configureClient = null)
    {
        services.AddKiotaHandlers();

        services
            .AddHttpClient<GymDiaryApiClientFactory>((_, http) => configureClient?.Invoke(http))
            .AttachKiotaHandlers();

        services.AddTransient(sp => sp.GetRequiredService<GymDiaryApiClientFactory>().GetClient());

        return services;
    }
}
