namespace GymDiary.Application

#nowarn "20"

open System
open System.Runtime.CompilerServices
open GymDiary.Application.Time
open GymDiary.Application.ExerciseCategories
open Microsoft.Extensions.DependencyInjection

[<Extension>]
type ApplicationServicesExtensions() =
    [<Extension>]
    static member AddGymDiaryApplication(services: IServiceCollection) : IServiceCollection =
        services
            .AddSingleton<IClock>(SystemClock(TimeProvider.System))
            .AddSingleton<CreateExerciseCategoryWorkflow.Handler>()
