namespace GymDiary.Core.Domain.DomainTypes

open System

open FSharp.Data.UnitSystems.SI.UnitSymbols

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.CommonTypes

type ExerciseCategoryId = private ExerciseCategoryId of string
type ExerciseTemplateId = private ExerciseTemplateId of string
type WorkoutTemplateId = private WorkoutTemplateId of string
type WorkoutId = private WorkoutId of string
type SportsmanId = private SportsmanId of string

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
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: SportsmanId }

/// Workout template with description about a workout
type WorkoutTemplate =
    { Id: WorkoutTemplateId
      Name: String50
      Goal: String200 option
      Notes: String1k option
      Schedule: DayOfWeek Set
      Exercises: ExerciseTemplate list
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: SportsmanId }

/// Exercise completed on a particular date
type Exercise =
    { TemplateId: ExerciseTemplateId
      Sets: ExerciseSets
      StartedOn: DateTime
      CompletedOn: DateTime }

/// Workout completed on a particular date
type Workout =
    { Id: WorkoutId
      TemplateId: WorkoutTemplateId
      Exercises: Exercise list
      StartedOn: DateTime
      CompletedOn: DateTime
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

    let create fieldName value =
        ConstrainedType.createString fieldName ExerciseCategoryId (0, Int32.MaxValue) value

    let empty = create "Id" "" |> Result.getOk

module ExerciseTemplateId =

    let value (ExerciseTemplateId id) = id

    let create fieldName value =
        ConstrainedType.createString fieldName ExerciseTemplateId (0, Int32.MaxValue) value

    let empty = create "Id" "" |> Result.getOk

module WorkoutTemplateId =

    let value (WorkoutTemplateId id) = id

    let create fieldName value =
        ConstrainedType.createString fieldName WorkoutTemplateId (0, Int32.MaxValue) value

    let empty = create "Id" "" |> Result.getOk

module WorkoutId =

    let value (WorkoutId id) = id

    let create fieldName value =
        ConstrainedType.createString fieldName WorkoutId (0, Int32.MaxValue) value

    let empty = create "Id" "" |> Result.getOk

module SportsmanId =

    let value (SportsmanId id) = id

    let create fieldName value =
        ConstrainedType.createString fieldName SportsmanId (0, Int32.MaxValue) value

    let empty = create "Id" "" |> Result.getOk

module EquipmentWeightKg =

    let value (EquipmentWeightKg value) = decimal value

    let create (fieldName: string) (value: decimal) =
        let valueKg = LanguagePrimitives.DecimalWithMeasure<kg> value
        ConstrainedType.createDecimalKg fieldName EquipmentWeightKg (0.1M<kg>, 1000M<kg>) valueKg

module RepsSet =

    let create orderNum reps : RepsSet =
        { OrderNum = orderNum
          Reps = reps }

module RepsWeightSet =

    let create orderNum reps equipmentWeight : RepsWeightSet =
        { OrderNum = orderNum
          Reps = reps
          EquipmentWeight = equipmentWeight }

module DurationSet =

    let create orderNum duration : DurationSet =
        { OrderNum = orderNum
          Duration = duration }

module DurationWeightSet =

    let create orderNum duration equipmentWeight : DurationWeightSet =
        { OrderNum = orderNum
          Duration = duration
          EquipmentWeight = equipmentWeight }

module DurationDistanceSet =

    let create orderNum duration distance : DurationDistanceSet =
        { OrderNum = orderNum
          Duration = duration
          Distance = distance }

module ExerciseCategory =

    let create id name ownerId : ExerciseCategory =
        { Id = id
          Name = name
          OwnerId = ownerId }

module ExerciseTemplate =
    let create id categoryId name notes restTime sets createdOn lastModifiedOn ownerId : ExerciseTemplate =
        { Id = id
          CategoryId = categoryId
          Name = name
          Notes = notes
          RestTime = restTime
          Sets = sets
          CreatedOn = createdOn
          LastModifiedOn = lastModifiedOn
          OwnerId = ownerId }

module WorkoutTemplate =
    let create id name goal notes schedule exercises createdOn lastModifiedOn ownerId : WorkoutTemplate =
        { Id = id
          Name = name
          Goal = goal
          Notes = notes
          Schedule = schedule
          Exercises = exercises
          CreatedOn = createdOn
          LastModifiedOn = lastModifiedOn
          OwnerId = ownerId }

module Exercise =
    let create templateId sets startedOn completedOn : Exercise =
        { TemplateId = templateId
          Sets = sets
          StartedOn = startedOn
          CompletedOn = completedOn }

module Workout =
    let create id templateId exercises startedOn completedOn ownerId : Workout =
        { Id = id
          TemplateId = templateId
          Exercises = exercises
          StartedOn = startedOn
          CompletedOn = completedOn
          OwnerId = ownerId }

module Sportsman =
    let create id email firstName lastName dateOfBirth sex : Sportsman =
        { Id = id
          Email = email
          FirstName = firstName
          LastName = lastName
          DateOfBirth = dateOfBirth
          Sex = sex }
