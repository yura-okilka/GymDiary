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
            "/v1/users/{userId}/exercise-categories",
            Func<_, _, _>(fun userId request -> ExerciseCategoryHandler.Create(root.CreateExerciseCategory, userId, request))
        )

        categories.MapGet(
            "/v1/users/{userId}/exercise-categories",
            Func<_, _>(fun userId -> ExerciseCategoryHandler.GetAll(root.GetAllExerciseCategories, userId))
        )

        categories.MapGet(
            "/v1/users/{userId}/exercise-categories/{categoryId}",
            Func<_, _, _>(fun userId categoryId -> ExerciseCategoryHandler.GetById(root.GetExerciseCategory, userId, categoryId))
        )

        categories.MapPut(
            "/v1/users/{userId}/exercise-categories/{categoryId}",
            Func<_, _, _, _>(fun userId categoryId request ->
                ExerciseCategoryHandler.Rename(root.RenameExerciseCategory, userId, categoryId, request))
        )

        categories.MapDelete(
            "/v1/users/{userId}/exercise-categories/{categoryId}",
            Func<_, _, _>(fun userId categoryId ->
                ExerciseCategoryHandler.Delete(root.DeleteExerciseCategory, userId, categoryId))
        )

        app
            .MapPost(
                "/v1/users/{userId}/exercises",
                Func<_, _, _>(fun userId request -> ExerciseHandler.Create(root.CreateExercise, userId, request))
            )
            .WithTags("Exercises")

        app
            .MapPost("/v1/users", Func<_, _>(fun request -> UserHandler.Create(root.CreateUser, request)))
            .WithTags("Users")

        app.MapGet("/ping", Func<_>(fun () -> "pong")).WithTags("Ping")

        app
