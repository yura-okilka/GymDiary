namespace GymDiary.Api.DependencyInjection

open GymDiary.Api.DependencyInjection
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ExerciseCategory

type CompositionRoot =
    { CreateExerciseCategory: CreateExerciseCategory.Workflow
      GetAllExerciseCategories: GetAllExerciseCategories.Workflow
      GetExerciseCategory: GetExerciseCategory.Workflow
      RenameExerciseCategory: RenameExerciseCategory.Workflow
      DeleteExerciseCategory: DeleteExerciseCategory.Workflow }

module CompositionRoot =

    let compose (trunk: Trunk) =
        let createExerciseCategoryWorkflow =
            ErrorLoggingDecorator.logWorkflow
                trunk.Logger
                CreateExerciseCategory.LoggingContext
                (CreateExerciseCategory.runWorkflow
                    trunk.Persistence.ExerciseCategory.ExistWithName
                    trunk.Persistence.Sportsman.ExistWithId
                    trunk.Persistence.ExerciseCategory.Create
                    trunk.Logger)

        let getAllExerciseCategoriesWorkflow =
            GetAllExerciseCategories.runWorkflow trunk.Persistence.ExerciseCategory.GetAll

        let getExerciseCategoryWorkflow =
            GetExerciseCategory.runWorkflow trunk.Persistence.ExerciseCategory.GetById

        let renameExerciseCategoryWorkflow =
            ErrorLoggingDecorator.logWorkflow
                trunk.Logger
                RenameExerciseCategory.LoggingContext
                (RenameExerciseCategory.runWorkflow
                    trunk.Persistence.ExerciseCategory.GetById
                    trunk.Persistence.ExerciseCategory.ExistWithName
                    trunk.Persistence.ExerciseCategory.Update
                    trunk.Logger)

        let deleteExerciseCategoryWorkflow =
            ErrorLoggingDecorator.logWorkflow
                trunk.Logger
                DeleteExerciseCategory.LoggingContext
                (DeleteExerciseCategory.runWorkflow trunk.Persistence.ExerciseCategory.Delete trunk.Logger)

        { CreateExerciseCategory = createExerciseCategoryWorkflow
          GetAllExerciseCategories = getAllExerciseCategoriesWorkflow
          GetExerciseCategory = getExerciseCategoryWorkflow
          RenameExerciseCategory = renameExerciseCategoryWorkflow
          DeleteExerciseCategory = deleteExerciseCategoryWorkflow }
