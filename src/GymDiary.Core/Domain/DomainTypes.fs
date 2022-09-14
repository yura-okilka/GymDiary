namespace GymDiary.Core.Domain

open System

open FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module DomainTypes =

    type Sportsman =
        {
            Id: Id<Sportsman>
            Email: EmailAddress
            FirstName: String50
            LastName: String50
            DateOfBirth: DateOnly option
            Gender: Gender option
        }

    /// Exercise set represented by reps quantity
    type RepsSet =
        {
            OrderNum: PositiveInt
            Reps: PositiveInt
        }

    /// Exercise set represented by reps quantity & equipment weight
    type RepsWeightSet =
        {
            OrderNum: PositiveInt
            Reps: PositiveInt
            EquipmentWeight: float<kg>
        }

    /// Exercise set represented by exercise duration
    type DurationSet =
        {
            OrderNum: PositiveInt
            Duration: TimeSpan
        }

    /// Exercise set represented by exercise duration & equipment weight
    type DurationWeightSet =
        {
            OrderNum: PositiveInt
            Duration: TimeSpan
            EquipmentWeight: float<kg>
        }

    /// Exercise set represented by exercise duration & distance
    type DurationDistanceSet =
        {
            OrderNum: PositiveInt
            Duration: TimeSpan
            Distance: decimal<m>
        }

    type ExerciseSets =
        | RepsSets of RepsSet list
        | RepsWeightSets of RepsWeightSet list
        | DurationSets of DurationSet list
        | DurationWeightSets of DurationWeightSet list
        | DurationDistanceSets of DurationDistanceSet list

    type ExerciseCategory =
        {
            Id: Id<ExerciseCategory>
            Name: String50
            OwnerId: Id<Sportsman>
        }

    /// Exercise template with description about an exercise
    type Exercise =
        {
            Id: Id<Exercise>
            CategoryId: Id<ExerciseCategory>
            Name: String50
            Notes: String1k option
            RestTime: TimeSpan
            Sets: ExerciseSets
            CreatedOn: DateTime
            LastModifiedOn: DateTime
            OwnerId: Id<Sportsman>
        }

    /// Routine with workout description
    type Routine =
        {
            Id: Id<Routine>
            Name: String50
            Goal: String200 option
            Notes: String1k option
            Schedule: DayOfWeek Set
            Exercises: Exercise list
            CreatedOn: DateTime
            LastModifiedOn: DateTime
            OwnerId: Id<Sportsman>
        }

    /// Exercise session completed on a particular date
    type ExerciseSession =
        {
            ExerciseId: Id<Exercise>
            Sets: ExerciseSets
            StartedOn: DateTime
            CompletedOn: DateTime
        }

    /// Workout session completed on a particular date
    type WorkoutSession =
        {
            Id: Id<WorkoutSession>
            RoutineId: Id<Routine>
            Exercises: ExerciseSession list
            StartedOn: DateTime
            CompletedOn: DateTime
            OwnerId: Id<Sportsman>
        }

    type SportsmanId = Id<Sportsman>
    type ExerciseCategoryId = Id<ExerciseCategory>
    type ExerciseId = Id<Exercise>
    type RoutineId = Id<Routine>
    type WorkoutSessionId = Id<WorkoutSession>
