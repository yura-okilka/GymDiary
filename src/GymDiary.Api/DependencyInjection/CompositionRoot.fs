namespace GymDiary.Api.DependencyInjection

open System
open GymDiary.Core.Persistence
open Microsoft.Extensions.DependencyInjection
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.Exercise
open GymDiary.Core.Workflows.ExerciseCategory
open Microsoft.Extensions.Logging

type CompositionRoot = {
    CreateExerciseCategory: CreateExerciseCategory.Workflow
    GetAllExerciseCategories: GetAllExerciseCategories.Workflow
    GetExerciseCategory: GetExerciseCategory.Workflow
    RenameExerciseCategory: RenameExerciseCategory.Workflow
    DeleteExerciseCategory: DeleteExerciseCategory.Workflow
    CreateExercise: CreateExercise.Workflow
}

module CompositionRoot =

    let compose (sp: IServiceProvider) =
        let logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger()

        let createExerciseCategoryWorkflow =
            CreateExerciseCategory.execute
                (sp.GetRequiredService<IExerciseCategoryRepository>().ExistWithName)
                (sp.GetRequiredService<ISportsmanRepository>().ExistWithId)
                (sp.GetRequiredService<IExerciseCategoryRepository>().Create)
                logger

        let getAllExerciseCategoriesWorkflow =
            GetAllExerciseCategories.execute (sp.GetRequiredService<IExerciseCategoryRepository>().GetAll)

        let getExerciseCategoryWorkflow =
            GetExerciseCategory.execute (sp.GetRequiredService<IExerciseCategoryRepository>().GetById)

        let renameExerciseCategoryWorkflow =
            RenameExerciseCategory.execute
                (sp.GetRequiredService<IExerciseCategoryRepository>().GetById)
                (sp.GetRequiredService<IExerciseCategoryRepository>().ExistWithName)
                (sp.GetRequiredService<IExerciseCategoryRepository>().Update)
                logger

        let deleteExerciseCategoryWorkflow =
            DeleteExerciseCategory.execute (sp.GetRequiredService<IExerciseCategoryRepository>().Delete) logger

        let errorLoggingDecorator loggingContext workflow =
            ErrorLoggingDecorator.logWorkflow logger loggingContext workflow

        let createExerciseWorkflow =
            CreateExercise.execute
                (sp.GetRequiredService<IExerciseCategoryRepository>().GetById)
                (sp.GetRequiredService<ISportsmanRepository>().ExistWithId)
                (sp.GetRequiredService<IExerciseRepository>().Create)
                logger

        {
            CreateExerciseCategory =
                createExerciseCategoryWorkflow |> errorLoggingDecorator CreateExerciseCategory.LoggingInfoProvider
            GetAllExerciseCategories = getAllExerciseCategoriesWorkflow
            GetExerciseCategory = getExerciseCategoryWorkflow
            RenameExerciseCategory =
                renameExerciseCategoryWorkflow |> errorLoggingDecorator RenameExerciseCategory.LoggingInfoProvider
            DeleteExerciseCategory =
                deleteExerciseCategoryWorkflow |> errorLoggingDecorator DeleteExerciseCategory.LoggingInfoProvider
            CreateExercise = createExerciseWorkflow |> errorLoggingDecorator CreateExercise.LoggingInfoProvider
        }
