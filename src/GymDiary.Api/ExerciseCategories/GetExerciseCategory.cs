using GymDiary.Api.Endpoints;

namespace GymDiary.Api.ExerciseCategories;

public class GetExerciseCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercise-categories/{id}", (string id) => Task.FromResult(TypedResults.Ok(new { id = $"sample-{id}" })))
            .WithName(nameof(GetExerciseCategory))
            .WithTags(EndpointTags.ExerciseCategories);
    }
}
