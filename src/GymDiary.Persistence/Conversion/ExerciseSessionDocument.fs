namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Core.Domain.WorkoutAggregate
open GymDiary.Persistence
open FsToolkit.ErrorHandling

module ExerciseSessionDocument =

    let fromDomain (domain: Exercise) : ExerciseSessionDocument =
        let (setType, sets) = ExerciseSetDocument.fromExerciseSets domain.Sets

        {
            ExerciseId = domain.ExerciseDefinitionId |> Id.value
            SetsType = setType
            Sets = sets
            StartedOn = domain.StartedOn
            CompletedOn = domain.CompletedOn
        }

    let toDomain (document: ExerciseSessionDocument) : Result<Exercise, ValidationError> = result {
        let! exerciseId = document.ExerciseId |> Id.tryCreate (nameof document.ExerciseId)
        let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType

        return ExerciseSession.create exerciseId sets document.StartedOn document.CompletedOn
    }
