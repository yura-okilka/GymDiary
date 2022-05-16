namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseDto =

    let fromDomain (domain: Exercise) : ExerciseDto =
        { TemplateId = domain.TemplateId |> ExerciseTemplateId.value
          Sets = domain.Sets |> ExerciseSetsDto.fromDomain
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (dto: ExerciseDto) : Result<Exercise, ValidationError> =
        result {
            let! templateId = dto.TemplateId |> ExerciseTemplateId.create (nameof dto.TemplateId)
            let! sets = dto.Sets |> ExerciseSetsDto.toDomain

            return Exercise.create templateId sets dto.StartedOn dto.CompletedOn
        }
