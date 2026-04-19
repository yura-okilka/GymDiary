using System.Text.Json.Nodes;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GymDiary.Api.OpenApi;

public sealed class ProblemResponseTransformer : IOpenApiOperationTransformer
{
    private const string MediaType = "application/problem+json";

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        foreach (var meta in context.Description.ActionDescriptor.EndpointMetadata.OfType<ProblemResponseMetadata>())
        {
            if (operation.Responses is null ||
                !operation.Responses.TryGetValue(meta.StatusCode.ToString(), out var response))
            {
                continue;
            }

            if (meta.Description is not null)
            {
                response.Description = meta.Description;
            }

            if (meta.Examples is null ||
                response.Content is null ||
                !response.Content.TryGetValue(MediaType, out var media))
            {
                continue;
            }

            media.Examples = meta.Examples.ToDictionary<KeyValuePair<string, ProblemResponseExample>, string, IOpenApiExample>(
                kv => kv.Key,
                kv => new OpenApiExample
                {
                    Summary = kv.Value.Summary,
                    Value = JsonNode.Parse(kv.Value.Json)
                });
        }

        return Task.CompletedTask;
    }
}
