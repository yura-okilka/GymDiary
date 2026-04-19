using GymDiary.Api.Endpoints;
using GymDiary.Application.Authentication;
using GymDiary.Application.ExerciseCategories;

using static GymDiary.Application.ExerciseCategories.CreateExerciseCategoryWorkflow.ResultExtensions;

namespace GymDiary.Api.ExerciseCategories;

public class CreateExerciseCategoryEndpoint : IEndpoint
{
    public record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("exercise-categories",
                async (Request request, CreateExerciseCategoryWorkflow.Handler handler, IUserContext userContext) =>
                {
                    var result = await handler.Handle(new CreateExerciseCategoryWorkflow.Command(request.Name, userContext.UserId));

                    return result.Match<IResult>(
                        id => TypedResults.CreatedAtRoute(id, nameof(GetExerciseCategory), new RouteValueDictionary(new { id })),
                        onInvalidCommand: e => TypedResults.BadRequest(),
                        onCategoryAlreadyExists: e => TypedResults.Conflict(),
                        onOwnerNotFound: e => TypedResults.Conflict());
                })
            .WithName(nameof(CreateExerciseCategoryEndpoint))
            .WithTags(EndpointTags.ExerciseCategories);
}
