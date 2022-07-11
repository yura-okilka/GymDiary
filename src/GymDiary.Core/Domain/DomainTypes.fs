namespace GymDiary.Core.Domain

open System

open FSharp.Data.UnitSystems.SI.UnitSymbols

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

type Sportsman =
    { Id: Id<Sportsman>
      Email: EmailAddress
      FirstName: String50
      LastName: String50
      DateOfBirth: DateOnly option
      Gender: Gender option }

type ExerciseCategory =
    { Id: Id<ExerciseCategory>
      Name: String50
      OwnerId: Id<Sportsman> }

/// Exercise template with description about an exercise
type Exercise =
    { Id: Id<Exercise>
      CategoryId: Id<ExerciseCategory>
      Name: String50
      Notes: String1k option
      RestTime: TimeSpan
      Sets: ExerciseSets
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: Id<Sportsman> }

/// Routine with workout description
type Routine =
    { Id: Id<Routine>
      Name: String50
      Goal: String200 option
      Notes: String1k option
      Schedule: DayOfWeek Set
      Exercises: Exercise list
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: Id<Sportsman> }

/// Exercise session completed on a particular date
type ExerciseSession =
    { ExerciseId: Id<Exercise>
      Sets: ExerciseSets
      StartedOn: DateTime
      CompletedOn: DateTime }

/// Workout session completed on a particular date
type WorkoutSession =
    { Id: Id<WorkoutSession>
      RoutineId: Id<Routine>
      Exercises: ExerciseSession list
      StartedOn: DateTime
      CompletedOn: DateTime
      OwnerId: Id<Sportsman> }

type SportsmanId = Id<Sportsman>
type ExerciseCategoryId = Id<ExerciseCategory>
type ExerciseId = Id<Exercise>
type RoutineId = Id<Routine>
type WorkoutSessionId = Id<WorkoutSession>

module EquipmentWeightKg =

    open Common.Extensions

    let create (fieldName: string) (value: decimal) =
        ConstrainedType.createDecimalKg fieldName EquipmentWeightKg (0.1M<kg>, 1000M<kg>) (decimalKg value)

    let value (EquipmentWeightKg value) = decimal value

module RepsSet =

    let create orderNum reps : RepsSet = { OrderNum = orderNum; Reps = reps }

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

    let rename (name: String50) (category: ExerciseCategory) = { category with Name = name }

module Exercise =

    let create id categoryId name notes restTime sets createdOn lastModifiedOn ownerId : Exercise =
        { Id = id
          CategoryId = categoryId
          Name = name
          Notes = notes
          RestTime = restTime
          Sets = sets
          CreatedOn = createdOn
          LastModifiedOn = lastModifiedOn
          OwnerId = ownerId }

module Routine =

    let create id name goal notes schedule exercises createdOn lastModifiedOn ownerId : Routine =
        { Id = id
          Name = name
          Goal = goal
          Notes = notes
          Schedule = schedule
          Exercises = exercises
          CreatedOn = createdOn
          LastModifiedOn = lastModifiedOn
          OwnerId = ownerId }

module ExerciseSession =

    let create exerciseId sets startedOn completedOn : ExerciseSession =
        { ExerciseId = exerciseId
          Sets = sets
          StartedOn = startedOn
          CompletedOn = completedOn }

module WorkoutSession =

    let create id routineId exercises startedOn completedOn ownerId : WorkoutSession =
        { Id = id
          RoutineId = routineId
          Exercises = exercises
          StartedOn = startedOn
          CompletedOn = completedOn
          OwnerId = ownerId }

module Sportsman =

    let create id email firstName lastName dateOfBirth gender : Sportsman =
        { Id = id
          Email = email
          FirstName = firstName
          LastName = lastName
          DateOfBirth = dateOfBirth
          Gender = gender }
