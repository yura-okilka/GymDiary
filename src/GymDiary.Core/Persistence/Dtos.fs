namespace GymDiary.Core.Persistence.Dtos

open System
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open FsToolkit.ErrorHandling.Operator.Result

type RepsSetDto = {
    OrderNum: int
    Reps: int
}

type RepsWeightSetDto = {
    OrderNum: int
    Reps: int
    EquipmentWeight: decimal
}

type DurationSetDto = {
    OrderNum: int
    Duration: TimeSpan
}

type DurationWeightSetDto = {
    OrderNum: int
    Duration: TimeSpan
    EquipmentWeight: decimal
}

type DurationDistanceSetDto = {
    OrderNum: int
    Duration: TimeSpan
    Distance: decimal
}

type ExerciseSetsDtoDiscriminator = RepsSets | RepsWeightSets | DurationSets | DurationWeightSets | DurationDistanceSets

type ExerciseSetsDto = {
    Discriminator: ExerciseSetsDtoDiscriminator
    RepsSets: RepsSetDto list
    RepsWeightSets: RepsWeightSetDto list
    DurationSets: DurationSetDto list
    DurationWeightSets: DurationWeightSetDto list
    DurationDistanceSets: DurationDistanceSetDto list
}

type ExerciseCategoryDto = {
    Id: Guid
    Name: string
    OwnerId: Guid
}

type ExerciseTemplateDto = {
    Id: Guid
    CategoryId: Guid
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetsDto
    CreatedOn: DateTimeOffset
    LastModifiedOn: DateTimeOffset
    OwnerId: Guid
}

module ExerciseCategoryDto =

    let fromDomain (exerciseCategory: ExerciseCategory) : ExerciseCategoryDto =
        {
            Id = exerciseCategory.Id |> ExerciseCategoryId.value
            Name = exerciseCategory.Name |> String50.value
            OwnerId = exerciseCategory.OwnerId |> SportsmanId.value
        }

    let toDomain (exerciseCategoryDto: ExerciseCategoryDto) : Result<ExerciseCategory, string> =
        let id = exerciseCategoryDto.Id |> ExerciseCategoryId |> Ok
        let name = exerciseCategoryDto.Name |> String50.create "Exercise Category Name"
        let ownerId = exerciseCategoryDto.OwnerId |> SportsmanId |> Ok

        ExerciseCategory.create <!> id <*> name <*> ownerId
