namespace GymDiary.Api.CSharp;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapGymDiaryApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/ping", () => "pong");

        app.MapPost("/exercise-categories", () => "This is a POST");
        app.MapGet("/exercise-categories", () => "This is a GET all");
        app.MapGet("/exercise-categories/{categoryId}", (string categoryId) => $"This is a GET {categoryId}");
        app.MapPut("/exercise-categories/{categoryId}", (string categoryId) => $"This is a PUT {categoryId}");
        app.MapDelete("/exercise-categories/{categoryId}", (string categoryId) => $"This is a DELETE {categoryId}");

        app.MapPost("/exercises", () => "This is a POST");

        app.MapPost("/users", () => "This is a POST");

        return app;
    }
}
