using GymDiary.Application.Workflows.Validation;

namespace GymDiary.Api.Validation;

public static class ValidationErrorExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(e => e.field)
            .ToDictionary(g => g.Key, g => g.SelectMany(e => e.errors).ToArray());
}
