using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Api.Validation;
using GymDiary.Application.ExerciseCategories;
using GymDiary.Application.Identity;

using Microsoft.AspNetCore.Mvc;

namespace GymDiary.Api.ExerciseCategories;

public class RenameExerciseCategoryEndpoint : IEndpoint
{
    public record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("exercise-categories/{id:guid}",
                async (Guid id, Request request, RenameExerciseCategoryWorkflow.Handler handler, ICurrentUser currentUser) =>
                {
                    var result = await handler.Handle(
                        new RenameExerciseCategoryWorkflow.Command(id, currentUser.Id, request.Name));

                    return result.Match<IResult>(
                        _ => TypedResults.NoContent(),
                        onInvalidCommand: e => TypedResults.ValidationProblem(e.ToProblemDictionary()),
                        onCategoryNotFound: _ => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Exercise category not found",
                            Detail = $"Exercise category '{id}' was not found for this user."
                        }),
                        onCategoryAlreadyExists: e => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Exercise category already exists",
                            Detail = $"An exercise category named '{e.Value}' already exists for this user."
                        }));
                })
            .WithName(nameof(RenameExerciseCategoryEndpoint))
            .WithTags(EndpointTags.ExerciseCategories)
            .WithSummary("Rename an exercise category.")
            .WithDescription(
                "Renames an exercise category owned by the authenticated user. " +
                "Category names must be unique per owner.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithMetadata(new ProblemResponseMetadata(
                StatusCode: StatusCodes.Status400BadRequest,
                Description: "The request failed validation. 'errors' maps field names to messages."))
            .WithMetadata(new ProblemResponseMetadata(
                StatusCode: StatusCodes.Status404NotFound,
                Description: "The requested exercise category does not exist for this user.",
                Examples: new Dictionary<string, ProblemResponseExample>
                {
                    ["CategoryNotFound"] = new(
                        Summary: "No exercise category exists with this id for the user.",
                        Json: """
                              {
                                "status": 404,
                                "title": "Exercise category not found",
                                "detail": "Exercise category '0197e2a0-6b7a-7c3d-9e1f-2a3b4c5d6e7f' was not found for this user.",
                                "traceId": "00-abc123-..."
                              }
                              """)
                }))
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
                              """)
                }));
}
