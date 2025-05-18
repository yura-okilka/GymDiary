using GymDiary.Api.CSharp.Endpoints;

namespace GymDiary.Api.CSharp.ExerciseCategories;

public class GetExerciseCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercise-categories", () => Task.FromResult(TypedResults.Ok(new { items = new[] { new { id = "sample" } } })))
            .WithName(nameof(GetExerciseCategories))
            .WithTags(EndpointTags.ExerciseCategories);
    }
}
