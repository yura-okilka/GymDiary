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
            .AddSingleton<CreateExerciseCategory.ICommandHandler, CreateExerciseCategory.CommandHandler>()
            .AddSingleton<RenameExerciseCategory.ICommandHandler, RenameExerciseCategory.CommandHandler>()
            .AddSingleton<DeleteExerciseCategory.ICommandHandler, DeleteExerciseCategory.CommandHandler>()
            .AddSingleton<GetExerciseCategory.IQueryHandler, GetExerciseCategory.QueryHandler>()
            .AddSingleton<GetAllExerciseCategories.IQueryHandler, GetAllExerciseCategories.QueryHandler>()
            .AddSingleton<CreateExerciseDefinition.ICommandHandler, CreateExerciseDefinition.CommandHandler>()
            .AddSingleton<CreateUser.ICommandHandler, CreateUser.CommandHandler>()
