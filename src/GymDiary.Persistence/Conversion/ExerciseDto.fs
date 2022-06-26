namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseDto =

    let fromDomain (domain: Exercise) : ExerciseDto =
        { TemplateId = domain.TemplateId |> Id.value
          Sets = domain.Sets |> ExerciseSetsDto.fromDomain
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (dto: ExerciseDto) : Result<Exercise, ValidationError> =
        result {
            let! templateId = dto.TemplateId |> Id.create (nameof dto.TemplateId)
            let! sets = dto.Sets |> ExerciseSetsDto.toDomain

            return Exercise.create templateId sets dto.StartedOn dto.CompletedOn
        }
