namespace GymDiary.Api.Endpoints

#nowarn "20"

open System
open System.Runtime.CompilerServices
open GymDiary.Api.DependencyInjection
open GymDiary.Api.RouteHandlers
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Http

[<Extension>]
type EndpointRouteBuilderExtensions() =
    [<Extension>]
    static member MapGymDiaryApi(app: IEndpointRouteBuilder, root: CompositionRoot) : IEndpointRouteBuilder =
        let categories = app.MapGroup("/").WithTags("Exercise categories")

        categories.MapPost(
            "/v1/sportsmen/{sportsmanId}/exerciseCategories",
            Func<_, _, _>(fun sportsmanId request -> ExerciseCategoryHandler.Create(root.CreateExerciseCategory, sportsmanId, request))
        )

        categories.MapGet(
            "/v1/sportsmen/{sportsmanId}/exerciseCategories",
            Func<_, _>(fun sportsmanId -> ExerciseCategoryHandler.GetAll(root.GetAllExerciseCategories, sportsmanId))
        )

        categories.MapGet(
            "/v1/sportsmen/{sportsmanId}/exerciseCategories/{categoryId}",
            Func<_, _, _>(fun sportsmanId categoryId -> ExerciseCategoryHandler.GetById(root.GetExerciseCategory, sportsmanId, categoryId))
        )

        categories.MapPut(
            "/v1/sportsmen/{sportsmanId}/exerciseCategories/{categoryId}",
            Func<_, _, _, _>(fun sportsmanId categoryId request ->
                ExerciseCategoryHandler.Rename(root.RenameExerciseCategory, sportsmanId, categoryId, request))
        )

        categories.MapDelete(
            "/v1/sportsmen/{sportsmanId}/exerciseCategories/{categoryId}",
            Func<_, _, _>(fun sportsmanId categoryId ->
                ExerciseCategoryHandler.Delete(root.DeleteExerciseCategory, sportsmanId, categoryId))
        )

        app
            .MapPost(
                "/v1/sportsmen/{sportsmanId}/exercises",
                Func<_, _, _>(fun sportsmanId request -> ExerciseHandler.Create(root.CreateExercise, sportsmanId, request))
            )
            .WithTags("Exercises")

        app
            .MapPost("/v1/sportsmen", Func<_, _>(fun request -> SportsmanHandler.Create(root.CreateSportsman, request)))
            .WithTags("Sportsmen")

        app.MapGet("/ping", Func<_>(fun () -> "pong")).WithTags("Ping")

        app
