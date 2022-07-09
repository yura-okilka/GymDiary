namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseDocument =

    let fromDomain (domain: Exercise) : ExerciseDocument =
        let (setType, sets) = ExerciseSetDocument.fromExerciseSets domain.Sets

        { TemplateId = domain.TemplateId |> Id.value
          SetsType = setType
          Sets = sets
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (document: ExerciseDocument) : Result<Exercise, ValidationError> =
        result {
            let! templateId = document.TemplateId |> Id.create (nameof document.TemplateId)
            let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType

            return Exercise.create templateId sets document.StartedOn document.CompletedOn
        }
