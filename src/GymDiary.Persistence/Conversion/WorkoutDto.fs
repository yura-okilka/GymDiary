namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FSharpx.Collections

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module WorkoutDto =

    let fromDomain (domain: Workout) : WorkoutDto =
        { Id = domain.Id |> WorkoutId.value
          TemplateId = domain.TemplateId |> WorkoutTemplateId.value
          Exercises = domain.Exercises |> Seq.ofList |> Seq.map ExerciseDto.fromDomain |> ResizeArray<ExerciseDto>
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: WorkoutDto) : Result<Workout, ValidationError> =
        let id = dto.Id |> WorkoutId.create (nameof dto.Id)
        let templateId = dto.TemplateId |> WorkoutTemplateId.create (nameof dto.TemplateId)

        let exercises =
            if dto.Exercises = null then
                ValueNull(nameof dto.Exercises) |> Error
            else
                dto.Exercises |> ResizeArray.toList |> List.traverseResultM ExerciseDto.toDomain

        let startedOn = dto.StartedOn |> Ok
        let completedOn = dto.CompletedOn |> Ok
        let ownerId = dto.OwnerId |> SportsmanId.create (nameof dto.OwnerId)

        Workout.create <!> id <*> templateId <*> exercises <*> startedOn <*> completedOn <*> ownerId