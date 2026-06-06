namespace GymDiary.Application

#nowarn "20"

open System.Runtime.CompilerServices
open GymDiary.Application.ExerciseCategories
open Microsoft.Extensions.DependencyInjection

[<Extension>]
type ApplicationServiceExtensions() =
    [<Extension>]
    static member AddGymDiaryApplication(services: IServiceCollection) : IServiceCollection =
        services
            .AddSingleton<CreateExerciseCategoryWorkflow.Handler>()
            .AddSingleton<GetExerciseCategoryWorkflow.Handler>()
            .AddSingleton<GetAllExerciseCategoriesWorkflow.Handler>()
