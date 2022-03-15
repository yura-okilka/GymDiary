namespace GymDiary.Core.Persistence.Dtos

open System
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open FsToolkit.ErrorHandling.Operator.Result

type RepsSetDto =
    { OrderNum: int
      Reps: int }

type RepsWeightSetDto =
    { OrderNum: int
      Reps: int
      EquipmentWeight: decimal }

type DurationSetDto =
    { OrderNum: int
      Duration: TimeSpan }

type DurationWeightSetDto =
    { OrderNum: int
      Duration: TimeSpan
      EquipmentWeight: decimal }

type DurationDistanceSetDto =
    { OrderNum: int
      Duration: TimeSpan
      Distance: decimal }

// Enum for serialization
type ExerciseSetsDtoDiscriminator =
    | RepsSets = 1
    | RepsWeightSets = 2
    | DurationSets = 3
    | DurationWeightSets = 4
    | DurationDistanceSets = 5

type ExerciseSetsDto =
    { Discriminator: ExerciseSetsDtoDiscriminator
      RepsSets: RepsSetDto list
      RepsWeightSets: RepsWeightSetDto list
      DurationSets: DurationSetDto list
      DurationWeightSets: DurationWeightSetDto list
      DurationDistanceSets: DurationDistanceSetDto list }

type ExerciseCategoryDto =
    { Id: string
      Name: string
      OwnerId: string }

type ExerciseTemplateDto =
    { Id: string
      CategoryId: string
      Name: string
      Notes: string option
      RestTime: TimeSpan
      Sets: ExerciseSetsDto
      CreatedOn: DateTimeOffset
      LastModifiedOn: DateTimeOffset
      OwnerId: string }

module ExerciseCategoryDto =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDto =
        { Id = domain.Id |> ExerciseCategoryId.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: ExerciseCategoryDto) : Result<ExerciseCategory, string> =
        let id = dto.Id |> ExerciseCategoryId |> Ok
        let name = dto.Name |> String50.create "Exercise Category Name"
        let ownerId = dto.OwnerId |> SportsmanId |> Ok

        ExerciseCategory.create <!> id <*> name <*> ownerId
