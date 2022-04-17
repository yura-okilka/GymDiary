namespace GymDiary.Core

open GymDiary.Core.Persistence
open GymDiary.Core.Workflows.ExerciseCategory

type ICoreCompositionRoot =
    abstract member CreateExerciseCategoryWorkflow: CreateExerciseCategory.Workflow

module CoreRoot =

    let createRoot (persistenceRoot: IPersistenceCompositionRoot) =
        let createExerciseCategoryWorkflow =
            CreateExerciseCategory.createWorkflow
                persistenceRoot.ExerciseCategoryRepository.ExistWithName
                persistenceRoot.SportsmanRepository.ExistWithId
                persistenceRoot.ExerciseCategoryRepository.Create

        { new ICoreCompositionRoot with
            member _.CreateExerciseCategoryWorkflow = createExerciseCategoryWorkflow }
