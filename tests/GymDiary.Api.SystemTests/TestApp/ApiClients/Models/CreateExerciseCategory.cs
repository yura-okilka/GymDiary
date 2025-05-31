namespace GymDiary.Api.SystemTests.TestApp.ApiClients.Models;

public static class CreateExerciseCategory
{
    public record Request(string Name);

    public record Response(string Id);

    public static ErrorResponse OwnerNotFoundError(string id) => new(
        "OwnerNotFound",
        $"Owner with id {id} is not found",
        Details: null
    );
}
