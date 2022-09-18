namespace GymDiary.Api.DependencyInjection

open GymDiary.Api.DependencyInjection
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.Exercise
open GymDiary.Core.Workflows.ExerciseCategory

type CompositionRoot =
    {
        CreateExerciseCategory: CreateExerciseCategory.Workflow
        GetAllExerciseCategories: GetAllExerciseCategories.Workflow
        GetExerciseCategory: GetExerciseCategory.Workflow
        RenameExerciseCategory: RenameExerciseCategory.Workflow
        DeleteExerciseCategory: DeleteExerciseCategory.Workflow
        CreateExercise: CreateExercise.Workflow
    }

module CompositionRoot =

    let compose (trunk: Trunk) =

        let createExerciseCategoryWorkflow =
            CreateExerciseCategory.execute
                trunk.Persistence.ExerciseCategory.ExistWithName
                trunk.Persistence.Sportsman.ExistWithId
                trunk.Persistence.ExerciseCategory.Create
                trunk.Logger

        let getAllExerciseCategoriesWorkflow =
            GetAllExerciseCategories.execute trunk.Persistence.ExerciseCategory.GetAll

        let getExerciseCategoryWorkflow = GetExerciseCategory.execute trunk.Persistence.ExerciseCategory.GetById

        let renameExerciseCategoryWorkflow =
            RenameExerciseCategory.execute
                trunk.Persistence.ExerciseCategory.GetById
                trunk.Persistence.ExerciseCategory.ExistWithName
                trunk.Persistence.ExerciseCategory.Update
                trunk.Logger

        let deleteExerciseCategoryWorkflow =
            DeleteExerciseCategory.execute trunk.Persistence.ExerciseCategory.Delete trunk.Logger

        let errorLoggingDecorator loggingContext workflow =
            ErrorLoggingDecorator.logWorkflow trunk.Logger loggingContext workflow

        let createExerciseWorkflow =
            CreateExercise.execute
                trunk.Persistence.ExerciseCategory.GetById
                trunk.Persistence.Sportsman.ExistWithId
                trunk.Persistence.Exercise.Create
                trunk.Logger

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
