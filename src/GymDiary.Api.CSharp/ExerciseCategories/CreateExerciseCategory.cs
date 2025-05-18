using GymDiary.Api.CSharp.Endpoints;

namespace GymDiary.Api.CSharp.ExerciseCategories;

public class CreateExerciseCategory : IEndpoint
{
    public record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("exercise-categories",
                (Request request) => Task.FromResult(TypedResults.CreatedAtRoute(
                    request,
                    nameof(GetExerciseCategory),
                    new RouteValueDictionary(new { id = "sample" }))))
            .WithName(nameof(CreateExerciseCategory))
            .WithTags(EndpointTags.ExerciseCategories);
    }
}
