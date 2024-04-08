using System.Net;
using System.Text.Json;

using FluentAssertions;
using FluentAssertions.Execution;

using Refit;

namespace SystemTests.Assertions;

/// <summary>
///     Assertion methods for Refit <see cref="IApiResponse" />
/// </summary>
public static class ApiResponseExtensions
{
    public static async Task ShouldHaveSuccess(this IApiResponse response)
    {
        using var _ = new AssertionScope(); // for throwing one exception with all failures

        response.IsSuccessStatusCode.Should().Be(true);
        await response.ShouldNotHaveError();
    }

    public static async Task ShouldHaveSuccess(this IApiResponse response, HttpStatusCode statusCode)
    {
        using var _ = new AssertionScope(); // for throwing one exception with all failures

        response.StatusCode.Should().Be(statusCode);
        await response.ShouldNotHaveError();
    }

    public static async Task ShouldHaveSuccess<TContent>(
        this IApiResponse<TContent> response,
        HttpStatusCode statusCode,
        TContent expectedContent
    )
    {
        using var _ = new AssertionScope(); // for throwing one exception with all failures

        response.StatusCode.Should().Be(statusCode);
        await response.ShouldNotHaveError();
        response.Content.Should().BeEquivalentTo(expectedContent);
    }

    public static async Task ShouldHaveError<TError>(this IApiResponse response, HttpStatusCode statusCode, TError expectedError)
    {
        using var _ = new AssertionScope(); // for throwing one exception with all failures

        response.StatusCode.Should().Be(statusCode);
        response.Error?.Content.Should().NotBeNull();

        var actualError = await response.Error!.GetContentAsAsync<TError>();
        actualError.Should().BeEquivalentTo(expectedError);
    }

    private static async Task ShouldNotHaveError(this IApiResponse response)
    {
        response.Error.Should().BeNull();

        if (response.Error?.Content is not null)
        {
            // TODO: consider writing a custom IValueFormatter to write indented JSON. https://fluentassertions.com/extensibility/#rendering-objects-with-beauty
            using var jsonContent = await response.Error.GetContentAsAsync<JsonDocument>();
            var errorContent = JsonSerializer.Serialize(jsonContent, new JsonSerializerOptions { WriteIndented = true });

            errorContent.Should().BeNull(); // for showing raw error response in the test output
        }
    }
}
