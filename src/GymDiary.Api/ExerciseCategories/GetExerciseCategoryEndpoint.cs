using GymDiary.Api.Contracts;
using GymDiary.Api.Endpoints;
using GymDiary.Api.OpenApi;
using GymDiary.Api.Validation;
using GymDiary.Application.ExerciseCategories;
using GymDiary.Application.Identity;

using Microsoft.AspNetCore.Mvc;

namespace GymDiary.Api.ExerciseCategories;

public class GetExerciseCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("exercise-categories/{id}",
                async (string id, GetExerciseCategoryWorkflow.Handler handler, ICurrentUser currentUser) =>
                {
                    var result = await handler.Handle(new GetExerciseCategoryWorkflow.Query(id, currentUser.Id));

                    return result.Match<IResult>(
                        category => TypedResults.Ok(new ExerciseCategoryResponse(
                            category.Id.Value,
                            category.Name.Value,
                            category.OwnerId.Value)),
                        onInvalidQuery: e => TypedResults.ValidationProblem(e.ToProblemDictionary()),
                        onCategoryNotFound: _ => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "Exercise category not found",
                            Detail = $"Exercise category '{id}' was not found for this user."
                        }));
                })
            .WithName(nameof(GetExerciseCategoryEndpoint))
            .WithTags(EndpointTags.ExerciseCategories)
            .WithSummary("Get an exercise category.")
            .WithDescription("Returns an exercise category owned by the authenticated user.")
            .Produces<ExerciseCategoryResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
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
                                "detail": "Exercise category '42' was not found for this user.",
                                "traceId": "00-abc123-..."
                              }
                              """)
                }));
}
