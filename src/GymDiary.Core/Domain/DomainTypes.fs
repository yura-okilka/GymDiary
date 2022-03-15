namespace GymDiary.Core.Domain.DomainTypes

open System
open GymDiary.Core.Domain.CommonTypes
open FSharp.Data.UnitSystems.SI.UnitSymbols

type ExerciseCategoryId = ExerciseCategoryId of string
type ExerciseTemplateId = ExerciseTemplateId of string
type WorkoutTemplateId = WorkoutTemplateId of string
type WorkoutId = WorkoutId of string
type SportsmanId = SportsmanId of string

/// Constrained to be a decimal kilogram between 0.1 and 1000.00
type EquipmentWeightKg = private EquipmentWeightKg of decimal<kg>

/// Exercise set represented by reps quantity
type RepsSet =
    { OrderNum: PositiveInt
      Reps: PositiveInt }

/// Exercise set represented by reps quantity & equipment weight
type RepsWeightSet =
    { OrderNum: PositiveInt
      Reps: PositiveInt
      EquipmentWeight: EquipmentWeightKg }

/// Exercise set represented by exercise duration
type DurationSet =
    { OrderNum: PositiveInt
      Duration: TimeSpan }

/// Exercise set represented by exercise duration & equipment weight
type DurationWeightSet =
    { OrderNum: PositiveInt
      Duration: TimeSpan
      EquipmentWeight: EquipmentWeightKg }

/// Exercise set represented by exercise duration & distance
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

/// Exercise template with description about an exercise
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

/// Workout template with description about a workout
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

/// Exercise completed on a particular date
type Exercise =
    { TemplateId: ExerciseTemplateId
      Sets: ExerciseSets
      StartedOn: DateTimeOffset
      CompletedOn: DateTimeOffset }

/// Workout completed on a particular date
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

module EquipmentWeightKg =

    let value (EquipmentWeightKg v) = v

    /// Create an EquipmentWeightKg from a decimal<kg>.
    /// Return Error if input is not a decimal<kg> between 0.1 and 1000.00
    let create fieldName v =
        ConstrainedType.createDecimalKg fieldName EquipmentWeightKg 0.1M<kg> 1000M<kg> v

module ExerciseCategory =

    let create id name ownerId : ExerciseCategory =
        { Id = id
          Name = name
          OwnerId = ownerId }
