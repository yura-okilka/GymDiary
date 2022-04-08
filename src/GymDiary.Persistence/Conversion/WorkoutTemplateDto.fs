namespace GymDiary.Persistence.Conversion

open System

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FSharpx.Collections

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module WorkoutTemplateDto =

    let fromDomain (domain: WorkoutTemplate) : WorkoutTemplateDto =
        { Id = domain.Id |> WorkoutTemplateId.value
          Name = domain.Name |> String50.value
          Goal = domain.Goal |> Option.map String200.value |> Option.defaultValue Unchecked.defaultof<string>
          Notes = domain.Notes |> Option.map String1k.value |> Option.defaultValue Unchecked.defaultof<string>
          Schedule = domain.Schedule |> ResizeArray<DayOfWeek>
          Exercises =
            domain.Exercises
            |> Seq.ofList
            |> Seq.map ExerciseTemplateDto.fromDomain
            |> ResizeArray<ExerciseTemplateDto>
          CreatedOn = domain.CreatedOn
          LastModifiedOn = domain.LastModifiedOn
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: WorkoutTemplateDto) : Result<WorkoutTemplate, ValidationError> =
        let id = dto.Id |> WorkoutTemplateId.create (nameof dto.Id)
        let name = dto.Name |> String50.create (nameof dto.Name)
        let goal = dto.Goal |> String200.createOption (nameof dto.Goal)
        let notes = dto.Notes |> String1k.createOption (nameof dto.Notes)

        let schedule =
            if dto.Schedule = null then
                ValueNull(nameof dto.Schedule) |> Error
            else
                dto.Schedule |> ResizeArray.toSeq |> Set.ofSeq |> Ok

        let exercises =
            if dto.Exercises = null then
                ValueNull(nameof dto.Exercises) |> Error
            else
                dto.Exercises |> ResizeArray.toList |> List.traverseResultM ExerciseTemplateDto.toDomain

        let createdOn = dto.CreatedOn |> Ok
        let lastModifiedOn = dto.LastModifiedOn |> Ok
        let ownerId = dto.OwnerId |> SportsmanId.create (nameof dto.OwnerId)

        WorkoutTemplate.create <!> id
        <*> name
        <*> goal
        <*> notes
        <*> schedule
        <*> exercises
        <*> createdOn
        <*> lastModifiedOn
        <*> ownerId
