namespace GymDiary.Api.CSharp;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapGymDiaryApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/ping", () => "pong");

        app.MapPost("/exerciseCategories", () => "This is a POST");
        app.MapGet("/exerciseCategories", () => "This is a GET all");
        app.MapGet("/exerciseCategories/{categoryId}", (string categoryId) => $"This is a GET {categoryId}");
        app.MapPut("/exerciseCategories/{categoryId}", (string categoryId) => $"This is a PUT {categoryId}");
        app.MapDelete("/exerciseCategories/{categoryId}", (string categoryId) => $"This is a DELETE {categoryId}");

        app.MapPost("/exercises", () => "This is a POST");

        app.MapPost("/sportsmen", () => "This is a POST");

        return app;
    }
}
