using GymDiary.ApiClient.Generated;

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace GymDiary.ApiClient;

/// <summary>
///     Factory responsible for creating <see cref="GymDiaryApiClient" /> instances.
///     Source: <see href="https://learn.microsoft.com/en-us/openapi/kiota/tutorials/dotnet-dependency-injection#create-a-client-factory" />.
/// </summary>
public class GymDiaryApiClientFactory(HttpClient httpClient)
{
    private readonly IAuthenticationProvider _authenticationProvider = new AnonymousAuthenticationProvider();

    public GymDiaryApiClient GetClient() => new(new HttpClientRequestAdapter(_authenticationProvider, httpClient: httpClient));
}
