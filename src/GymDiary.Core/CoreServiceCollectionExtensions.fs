namespace GymDiary.Core

#nowarn "20"

open System
open System.Runtime.CompilerServices
open GymDiary.Core.Time
open GymDiary.Core.Workflows.ExerciseDefinition
open GymDiary.Core.Workflows.ExerciseCategory
open GymDiary.Core.Workflows.User
open Microsoft.Extensions.DependencyInjection

[<Extension>]
type CoreServiceCollectionExtensions() =
    [<Extension>]
    static member AddCore(services: IServiceCollection) : IServiceCollection =
        services
            .AddSingleton<IClock>(SystemClock(TimeProvider.System))
            .AddSingleton<CreateExerciseCategory.CommandHandler>()
            .AddSingleton<RenameExerciseCategory.CommandHandler>()
            .AddSingleton<DeleteExerciseCategory.CommandHandler>()
            .AddSingleton<GetExerciseCategory.QueryHandler>()
            .AddSingleton<GetAllExerciseCategories.QueryHandler>()
            .AddSingleton<CreateExerciseDefinition.CommandHandler>()
            .AddSingleton<CreateUser.CommandHandler>()
