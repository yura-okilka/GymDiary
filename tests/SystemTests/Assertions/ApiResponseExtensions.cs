using System.Net;

using FluentAssertions;

using Refit;

namespace SystemTests.Assertions;

/// <summary>
///     Assertion methods for Refit <see cref="IApiResponse" />
/// </summary>
public static class ApiResponseExtensions
{
    public static void ShouldHaveSuccess<TContent>(
        this IApiResponse<TContent> response,
        HttpStatusCode statusCode,
        TContent expectedContent
    )
    {
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(statusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expectedContent);
    }

    public static async Task ShouldHaveError<TError>(this IApiResponse response, HttpStatusCode statusCode, TError expectedError)
    {
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(statusCode);
        response.Error.Should().NotBeNull();
        response.Error!.Content.Should().NotBeNull();

        var actualError = await response.Error!.GetContentAsAsync<TError>();
        actualError.Should().BeEquivalentTo(expectedError);
    }
}
