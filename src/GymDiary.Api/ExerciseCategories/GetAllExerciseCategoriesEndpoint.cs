using GymDiary.Api.Dtos;
using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Api.Validation;
using GymDiary.Application.ExerciseCategories;
using GymDiary.Application.Identity;

namespace GymDiary.Api.ExerciseCategories;

public class GetAllExerciseCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("exercise-categories",
                async (GetAllExerciseCategoriesWorkflow.Handler handler, ICurrentUser currentUser) =>
                {
                    var result = await handler.Handle(new GetAllExerciseCategoriesWorkflow.Query(currentUser.Id));

                    return result.Match<IResult>(
                        categories => TypedResults.Ok(categories
                            .Select(c => new ExerciseCategoryDto(c.Id.Value, c.Name.Value, c.OwnerId.Value))
                            .ToArray()),
                        onInvalidQuery: e => TypedResults.ValidationProblem(e.ToProblemDictionary()));
                })
            .WithName(nameof(GetAllExerciseCategoriesEndpoint))
            .WithTags(EndpointTags.ExerciseCategories)
            .WithSummary("Get all exercise categories.")
            .WithDescription("Returns all exercise categories owned by the authenticated user.")
            .Produces<ExerciseCategoryDto[]>()
            .ProducesValidationProblem()
            .WithMetadata(new ProblemResponseMetadata(
                StatusCode: StatusCodes.Status400BadRequest,
                Description: "The request failed validation. 'errors' maps field names to messages."));
}
