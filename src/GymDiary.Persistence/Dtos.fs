namespace GymDiary.Persistence.Dtos

open System

open FsToolkit.ErrorHandling.Operator.Result

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes

[<CLIMutable>]
type RepsSetDto =
    { OrderNum: int
      Reps: int }

[<CLIMutable>]
type RepsWeightSetDto =
    { OrderNum: int
      Reps: int
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationSetDto =
    { OrderNum: int
      Duration: TimeSpan }

[<CLIMutable>]
type DurationWeightSetDto =
    { OrderNum: int
      Duration: TimeSpan
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationDistanceSetDto =
    { OrderNum: int
      Duration: TimeSpan
      Distance: decimal }

// Tag to discriminate ExerciseSetsDtos
type ExerciseSetsDtoTag =
    | RepsSets = 1
    | RepsWeightSets = 2
    | DurationSets = 3
    | DurationWeightSets = 4
    | DurationDistanceSets = 5

[<CLIMutable>]
type ExerciseSetsDto =
    { Tag: ExerciseSetsDtoTag
      RepsSets: RepsSetDto list
      RepsWeightSets: RepsWeightSetDto list
      DurationSets: DurationSetDto list
      DurationWeightSets: DurationWeightSetDto list
      DurationDistanceSets: DurationDistanceSetDto list }

[<CLIMutable>]
type ExerciseCategoryDto =
    { Id: string
      Name: string
      OwnerId: string }

[<CLIMutable>]
type ExerciseTemplateDto =
    { Id: string
      CategoryId: string
      Name: string
      Notes: string option
      RestTime: TimeSpan
      Sets: ExerciseSetsDto
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type WorkoutTemplateDto =
    { Id: string
      Name: string
      Goal: string option
      Notes: string option
      Schedule: string list
      Exercises: ExerciseTemplateDto list
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type ExerciseDto =
    { TemplateId: string
      Sets: ExerciseSetsDto
      StartedOn: DateTime
      CompletedOn: DateTime }

[<CLIMutable>]
type WorkoutDto =
    { Id: string
      TemplateId: string
      Exercises: ExerciseDto list
      StartedOn: DateTime
      CompletedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type SportsmanDto =
    { Id: string
      Email: string
      FirstName: string
      LastName: string
      DateOfBirth: DateTime option
      Sex: string option }

module ExerciseCategoryDto =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDto =
        { Id = domain.Id |> ExerciseCategoryId.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: ExerciseCategoryDto) : Result<ExerciseCategory, ValidationError> =
        let id = dto.Id |> ExerciseCategoryId |> Ok
        let name = dto.Name |> String50.create "Exercise Category Name"
        let ownerId = dto.OwnerId |> SportsmanId |> Ok

        ExerciseCategory.create <!> id <*> name <*> ownerId
