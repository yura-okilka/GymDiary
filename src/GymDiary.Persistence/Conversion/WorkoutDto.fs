namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FSharpx.Collections

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module WorkoutDto =

    let fromDomain (domain: Workout) : WorkoutDto =
        { Id = domain.Id |> Id.value
          TemplateId = domain.TemplateId |> Id.value
          Exercises = domain.Exercises |> Seq.ofList |> Seq.map ExerciseDto.fromDomain |> ResizeArray<ExerciseDto>
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (dto: WorkoutDto) : Result<Workout, ValidationError> =
        let id = dto.Id |> Id.create (nameof dto.Id)
        let templateId = dto.TemplateId |> Id.create (nameof dto.TemplateId)

        let exercises =
            if dto.Exercises = null then
                ValidationError(nameof dto.Exercises, ValueNull) |> Error
            else
                dto.Exercises |> ResizeArray.toList |> List.traverseResultM ExerciseDto.toDomain

        let startedOn = dto.StartedOn |> Ok
        let completedOn = dto.CompletedOn |> Ok
        let ownerId = dto.OwnerId |> Id.create (nameof dto.OwnerId)

        Workout.create <!> id <*> templateId <*> exercises <*> startedOn <*> completedOn <*> ownerId
