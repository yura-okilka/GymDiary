namespace GymDiary.Persistence.Conversion

open System

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FSharpx.Collections

open FsToolkit.ErrorHandling

module WorkoutTemplateDto =

    let fromDomain (domain: WorkoutTemplate) : WorkoutTemplateDto =
        { Id = domain.Id |> Id.value
          Name = domain.Name |> String50.value
          Goal = domain.Goal |> Option.map String200.value |> Option.defaultValue defaultof<string>
          Notes = domain.Notes |> Option.map String1k.value |> Option.defaultValue defaultof<string>
          Schedule = domain.Schedule |> ResizeArray<DayOfWeek>
          Exercises =
            domain.Exercises
            |> Seq.ofList
            |> Seq.map ExerciseTemplateDto.fromDomain
            |> ResizeArray<ExerciseTemplateDto>
          CreatedOn = domain.CreatedOn
          LastModifiedOn = domain.LastModifiedOn
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (dto: WorkoutTemplateDto) : Result<WorkoutTemplate, ValidationError> =
        result {
            let! id = dto.Id |> Id.create (nameof dto.Id)
            let! name = dto.Name |> String50.create (nameof dto.Name)
            let! goal = dto.Goal |> String200.createOption (nameof dto.Goal)
            let! notes = dto.Notes |> String1k.createOption (nameof dto.Notes)

            let! schedule =
                if dto.Schedule = null then
                    ValidationError(nameof dto.Schedule, ValueNull) |> Error
                else
                    dto.Schedule |> ResizeArray.toSeq |> Set.ofSeq |> Ok

            let! exercises =
                if dto.Exercises = null then
                    ValidationError(nameof dto.Exercises, ValueNull) |> Error
                else
                    dto.Exercises |> ResizeArray.toList |> List.traverseResultM ExerciseTemplateDto.toDomain

            let! ownerId = dto.OwnerId |> Id.create (nameof dto.OwnerId)

            return WorkoutTemplate.create id name goal notes schedule exercises dto.CreatedOn dto.LastModifiedOn ownerId
        }
