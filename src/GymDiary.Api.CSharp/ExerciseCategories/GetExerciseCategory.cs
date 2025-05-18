using GymDiary.Api.CSharp.Endpoints;

namespace GymDiary.Api.CSharp.ExerciseCategories;

public class GetExerciseCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercise-categories/{id}", (string id) => Task.FromResult(TypedResults.Ok(new { id = $"sample-{id}" })))
            .WithName(nameof(GetExerciseCategory))
            .WithTags(EndpointTags.ExerciseCategories);
    }
}
