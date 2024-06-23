namespace GymDiary.Core.Domain

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module DomainTypes =

    type User = {
        Id: Id<User>
        Email: EmailAddress
        FirstName: String50
        LastName: String50
        DateOfBirth: DateOnly option
        Gender: Gender option
    }

    type ExerciseCategory = {
        Id: Id<ExerciseCategory>
        Name: String50
        OwnerId: Id<User>
    }

    type ExerciseSetType =
        | Repetitions
        | RepetitionsWithWeight
        | Duration
        | DurationWithWeight
        | DurationWithDistance

    type ExerciseSetData = {
        SequenceNumber: PositiveInt
        Repetitions: uint
        Weight: float<kg>
        Distance: float<m>
        Duration: TimeSpan
    }

    type ExerciseSets = {
        Type: ExerciseSetType
        Items: ExerciseSetData list
    }

    /// A template of an exercise
    type ExerciseDefinition = {
        Id: Id<ExerciseDefinition>
        CategoryId: Id<ExerciseCategory>
        Name: String50
        Notes: String1k option
        RestTime: TimeSpan
        Sets: ExerciseSets
        CreatedOn: DateTime
        LastModifiedOn: DateTime
        OwnerId: Id<User>
    }

    /// A template of a workout
    type Routine = {
        Id: Id<Routine>
        Name: String50
        Goal: String200 option
        Notes: String1k option
        Schedule: DayOfWeek Set
        Exercises: Id<ExerciseDefinition> Set
        CreatedOn: DateTime
        LastModifiedOn: DateTime
        OwnerId: Id<User>
    }

    /// A snapshot of exercise category at a particular time
    type ExerciseCategorySnapshot = {
        Id: Id<ExerciseCategory>
        Name: String50
        OwnerId: Id<User>
    }

    /// A snapshot of exercise definition at a particular time
    type ExerciseDefinitionSnapshot = {
        Id: Id<ExerciseDefinition>
        Category: ExerciseCategorySnapshot
        Name: String50
        Notes: String1k option
        RestTime: TimeSpan
        Sets: ExerciseSets
        OwnerId: Id<User>
    }

    /// A snapshot of routine at a particular time
    type RoutineSnapshot = {
        Id: Id<Routine>
        Name: String50
        Goal: String200 option
        Notes: String1k option
        Schedule: DayOfWeek Set
        OwnerId: Id<User>
    }

    /// An exercise completed at a particular time
    type Exercise = {
        Definition: ExerciseDefinitionSnapshot
        Sets: ExerciseSets
        StartedOn: DateTime
        CompletedOn: DateTime
    }

    /// A workout completed at a particular time
    type Workout = {
        Id: Id<Workout>
        Routine: RoutineSnapshot
        Exercises: Exercise list
        StartedOn: DateTime
        CompletedOn: DateTime
        OwnerId: Id<User>
    }

    type UserId = Id<User>
    type ExerciseCategoryId = Id<ExerciseCategory>
    type ExerciseDefinitionId = Id<ExerciseDefinition>
    type RoutineId = Id<Routine>
    type WorkoutId = Id<Workout>
