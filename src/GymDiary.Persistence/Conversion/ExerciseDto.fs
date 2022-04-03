namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling.Operator.Result

module ExerciseDto =

    let fromDomain (domain: Exercise) : ExerciseDto =
        { TemplateId = domain.TemplateId |> ExerciseTemplateId.value
          Sets = domain.Sets |> ExerciseSetsDto.fromDomain
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn }

    let toDomain (dto: ExerciseDto) : Result<Exercise, ValidationError> =
        let templateId = dto.TemplateId |> ExerciseTemplateId.create "TemplateId"
        let sets = dto.Sets |> ExerciseSetsDto.toDomain
        let startedOn = dto.StartedOn |> Ok
        let completedOn = dto.CompletedOn |> Ok

        Exercise.create <!> templateId <*> sets <*> startedOn <*> completedOn
