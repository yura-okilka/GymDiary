namespace GymDiary.Api.DependencyInjection

open GymDiary.Api.DependencyInjection
open GymDiary.Core.Workflows.ExerciseCategory

type CompositionRoot =
    { CreateExerciseCategory: CreateExerciseCategory.Workflow
      GetExerciseCategory: GetExerciseCategory.Workflow
      RenameExerciseCategory: RenameExerciseCategory.Workflow }

module CompositionRoot =

    let compose (trunk: Trunk) =
        let createExerciseCategoryWorkflow =
            CreateExerciseCategory.createWorkflow
                trunk.Persistence.ExerciseCategory.ExistWithName
                trunk.Persistence.Sportsman.ExistWithId
                trunk.Persistence.ExerciseCategory.Create

        let getExerciseCategoryWorkflow =
            GetExerciseCategory.createWorkflow trunk.Persistence.ExerciseCategory.GetById

        let renameExerciseCategoryWorkflow =
            RenameExerciseCategory.createWorkflow
                trunk.Persistence.ExerciseCategory.GetById
                trunk.Persistence.ExerciseCategory.ExistWithName
                trunk.Persistence.ExerciseCategory.Update

        { CreateExerciseCategory = createExerciseCategoryWorkflow
          GetExerciseCategory = getExerciseCategoryWorkflow
          RenameExerciseCategory = renameExerciseCategoryWorkflow }
