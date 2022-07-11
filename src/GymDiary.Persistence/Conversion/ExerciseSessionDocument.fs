namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseSessionDocument =

    let fromDomain (domain: ExerciseSession) : ExerciseSessionDocument =
        let (setType, sets) = ExerciseSetDocument.fromExerciseSets domain.Sets

        { TemplateId = domain.TemplateId |> Id.value
          SetsType = setType
          Sets = sets
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (document: ExerciseSessionDocument) : Result<ExerciseSession, ValidationError> =
        result {
            let! templateId = document.TemplateId |> Id.create (nameof document.TemplateId)
            let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType

            return ExerciseSession.create templateId sets document.StartedOn document.CompletedOn
        }
