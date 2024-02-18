using System.Net;
using FluentAssertions;
using Refit;

namespace SystemTests.Assertions;

public static class ApiResponseExtensions
{
    public static void ShouldBeSuccessful(this IApiResponse response) => ShouldBeSuccessfulInternal(response);

    public static void ShouldBeSuccessful<T>(this IApiResponse<T> response)
    {
        ShouldBeSuccessfulInternal(response);
        response.Content.Should().NotBeNull();
    }

    public static async Task ShouldHaveError<TContent, TError>(this IApiResponse<TContent> response, TError expectedError)
    {
        response.StatusCode.Should().Match(
            c => (int?)c >= (int)HttpStatusCode.BadRequest,
            "error HTTP code should be 4XX or 5XX"
        );
        response.IsSuccessStatusCode.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Content.Should().NotBeNull();

        var actualError = await response.Error!.GetContentAsAsync<TError>();
        actualError.Should().BeEquivalentTo(expectedError);
    }

    private static void ShouldBeSuccessfulInternal(this IApiResponse response)
    {
        // TODO: is IsSuccessStatusCode enough?
        response.StatusCode.Should().Match(
            code => (int?)code < (int)HttpStatusCode.BadRequest,
            "successful HTTP code should be 1XX, 2XX, or 3XX"
        );
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
