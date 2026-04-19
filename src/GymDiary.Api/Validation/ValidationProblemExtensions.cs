using GymDiary.Application.Workflows.Validation;

namespace GymDiary.Api.Validation;

public static class ValidationProblemExtensions
{
    public static IDictionary<string, string[]> ToProblemDictionary(this IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(e => e.field)
            .ToDictionary(g => g.Key, g => g.Select(e => e.error).ToArray());
}
