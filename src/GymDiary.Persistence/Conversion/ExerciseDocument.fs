namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseDocument =

    let fromDomain (domain: Exercise) : ExerciseDocument =
        { TemplateId = domain.TemplateId |> Id.value
          Sets = domain.Sets |> ExerciseSetsDocument.fromDomain
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (dto: ExerciseDocument) : Result<Exercise, ValidationError> =
        result {
            let! templateId = dto.TemplateId |> Id.create (nameof dto.TemplateId)
            let! sets = dto.Sets |> ExerciseSetsDocument.toDomain

            return Exercise.create templateId sets dto.StartedOn dto.CompletedOn
        }
