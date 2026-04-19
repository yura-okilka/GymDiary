using GymDiary.Api.Endpoints;
using GymDiary.Api.Validation;
using GymDiary.Application.Authentication;
using GymDiary.Application.ExerciseCategories;

using Microsoft.AspNetCore.Mvc;

using static GymDiary.Application.ExerciseCategories.CreateExerciseCategoryWorkflow.ResultExtensions;

namespace GymDiary.Api.ExerciseCategories;

public class CreateExerciseCategoryEndpoint : IEndpoint
{
    public record Request(string Name);

    public record Response(string Id);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("exercise-categories",
                async (Request request, CreateExerciseCategoryWorkflow.Handler handler, IUserContext userContext) =>
                {
                    var result = await handler.Handle(new CreateExerciseCategoryWorkflow.Command(request.Name, userContext.UserId));

                    return result.Match<IResult>(
                        id => TypedResults.CreatedAtRoute(new Response(id), nameof(GetExerciseCategory), new { id }),
                        onInvalidCommand: e => TypedResults.ValidationProblem(e.ToProblemDictionary()),
                        onCategoryAlreadyExists: e => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Exercise category already exists",
                            Detail = $"An exercise category named '{e.name}' already exists for this user."
                        }),
                        onOwnerNotFound: e => TypedResults.Problem(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Owner not found",
                            Detail = $"User '{e.id}' does not exist."
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
            .ProducesProblem(StatusCodes.Status409Conflict);
}
