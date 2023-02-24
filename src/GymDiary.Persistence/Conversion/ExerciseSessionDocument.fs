namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseSessionDocument =

    let fromDomain (domain: ExerciseSession) : ExerciseSessionDocument =
        let (setType, sets) = ExerciseSetDocument.fromExerciseSets domain.Sets

        {
            ExerciseId = domain.ExerciseId |> Id.value
            SetsType = setType
            Sets = sets
            StartedOn = domain.StartedOn
            CompletedOn = domain.CompletedOn
        }

    let toDomain (document: ExerciseSessionDocument) : Result<ExerciseSession, ValidationError> = result {
        let! exerciseId = document.ExerciseId |> Id.create (nameof document.ExerciseId)
        let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType

        return ExerciseSession.create exerciseId sets document.StartedOn document.CompletedOn
    }
