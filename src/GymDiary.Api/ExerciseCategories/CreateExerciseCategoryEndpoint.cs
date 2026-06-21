using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Api.Validation;
using GymDiary.Application.ExerciseCategories;
using GymDiary.Application.Identity;

using Microsoft.AspNetCore.Mvc;

namespace GymDiary.Api.ExerciseCategories;

public class CreateExerciseCategoryEndpoint : IEndpoint
{
    public record Request(string Name);

    public record Response(Guid Id);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("exercise-categories",
                async (Request request, CreateExerciseCategoryWorkflow.Handler handler, ICurrentUser currentUser) =>
                {
                    var result = await handler.Handle(new CreateExerciseCategoryWorkflow.Command(request.Name, currentUser.Id));

                    return result.Match<IResult>(
                        id => TypedResults.CreatedAtRoute(new Response(id), nameof(GetExerciseCategoryEndpoint), new { id }),
                        onInvalidCommand: e => TypedResults.ValidationProblem(e.ToProblemDictionary()),
                        onCategoryAlreadyExists: e => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Exercise category already exists",
                            Detail = $"An exercise category named '{e.Value}' already exists for this user."
                        }),
                        onOwnerNotFound: e => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Owner not found",
                            Detail = $"User '{e}' does not exist."
                        }));
                })
            .WithName(nameof(CreateExerciseCategoryEndpoint))
            .WithTags(EndpointTags.ExerciseCategories)
            .WithSummary("Create an exercise category.")
            .WithDescription(
                "Creates a new exercise category owned by the authenticated user. " +
                "Category names must be unique per owner.")
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithMetadata(new ProblemResponseMetadata(
                StatusCode: StatusCodes.Status400BadRequest,
                Description: "The request failed validation. 'errors' maps field names to messages."))
            .WithMetadata(new ProblemResponseMetadata(
                StatusCode: StatusCodes.Status409Conflict,
                Description: "The request conflicts with server state.",
                Examples: new Dictionary<string, ProblemResponseExample>
                {
                    ["CategoryAlreadyExists"] = new(
                        Summary: "A category with this name already exists for the user.",
                        Json: """
                              {
                                "status": 409,
                                "title": "Exercise category already exists",
                                "detail": "An exercise category named 'Push' already exists for this user.",
                                "traceId": "00-abc123-..."
                              }
                              """),
                    ["OwnerNotFound"] = new(
                        Summary: "The owning user does not exist.",
                        Json: """
                              {
                                "status": 409,
                                "title": "Owner not found",
                                "detail": "User '42' does not exist.",
                                "traceId": "00-abc123-..."
                              }
                              """)
                }));
}
