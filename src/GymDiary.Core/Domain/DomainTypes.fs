namespace GymDiary.Core.Domain.DomainTypes

open System
open GymDiary.Core.Domain.CommonTypes
open FSharp.Data.UnitSystems.SI.UnitSymbols

type ExerciseCategoryId = ExerciseCategoryId of Guid
type ExerciseTemplateId = ExerciseTemplateId of Guid
type WorkoutTemplateId = WorkoutTemplateId of Guid
type WorkoutId = WorkoutId of Guid
type SportsmanId = SportsmanId of Guid

type RepsSet =
    { OrderNum: PositiveInt
      Reps: PositiveInt }

type RepsWeightSet =
    { OrderNum: PositiveInt
      Reps: PositiveInt
      EquipmentWeight: EquipmentWeightKg }

type DurationSet =
    { OrderNum: PositiveInt
      Duration: TimeSpan }

type DurationWeightSet =
    { OrderNum: PositiveInt
      Duration: TimeSpan
      EquipmentWeight: EquipmentWeightKg }

type DurationDistanceSet =
    { OrderNum: PositiveInt
      Duration: TimeSpan
      Distance: decimal<m> }

type ExerciseSets =
    | RepsSets of RepsSet list
    | RepsWeightSets of RepsWeightSet list
    | DurationSets of DurationSet list
    | DurationWeightSets of DurationWeightSet list
    | DurationDistanceSets of DurationDistanceSet list

type ExerciseCategory =
    { Id: ExerciseCategoryId
      Name: String50
      OwnerId: SportsmanId }

type ExerciseTemplate =
    { Id: ExerciseTemplateId
      CategoryId: ExerciseCategoryId
      Name: String50
      Notes: String1k option
      RestTime: TimeSpan
      Sets: ExerciseSets
      CreatedOn: DateTimeOffset
      LastModifiedOn: DateTimeOffset
      OwnerId: SportsmanId }

type WorkoutTemplate =
    { Id: WorkoutTemplateId
      Name: String50
      Goal: String200 option
      Notes: String1k option
      Schedule: DayOfWeek Set
      Exercises: ExerciseTemplate list
      CreatedOn: DateTimeOffset
      LastModifiedOn: DateTimeOffset
      OwnerId: SportsmanId }

type Exercise =
    { TemplateId: ExerciseTemplateId
      Sets: ExerciseSets
      StartedOn: DateTimeOffset
      CompletedOn: DateTimeOffset }

type Workout =
    { Id: WorkoutId
      TemplateId: WorkoutTemplateId
      Exercises: Exercise list
      StartedOn: DateTimeOffset
      CompletedOn: DateTimeOffset
      OwnerId: SportsmanId }

type Sportsman =
    { Id: SportsmanId
      Email: EmailAddress
      FirstName: String50
      LastName: String50
      DateOfBirth: DateOnly option
      Sex: Sex option }

module ExerciseCategoryId =

    let value (ExerciseCategoryId id) = id

module ExerciseTemplateId =

    let value (ExerciseTemplateId id) = id

module WorkoutTemplateId =

    let value (WorkoutTemplateId id) = id

module WorkoutId =

    let value (WorkoutId id) = id

module SportsmanId =

    let value (SportsmanId id) = id

module ExerciseCategory =

    let create id name ownerId : ExerciseCategory =
        { Id = id
          Name = name
          OwnerId = ownerId }
