namespace GymDiary.Api.OpenApi;

public record ProblemResponseMetadata(
    int StatusCode,
    string? Description = null,
    IReadOnlyDictionary<string, ProblemResponseExample>? Examples = null);

public record ProblemResponseExample(string Summary, string Json);
