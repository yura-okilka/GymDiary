namespace GymDiary.Domain.ExerciseDefinitions

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

type ExerciseSetKind =
    | Repetitions
    | RepetitionsWithWeight
    | Duration
    | DurationWithWeight
    | DurationWithDistance

type ExerciseSet = {
    Kind: ExerciseSetKind
    Repetitions: uint
    Weight: float<kg>
    Distance: float<m>
    Duration: TimeSpan
} with

    static member ofRepetitions repetitions : ExerciseSet = {
        Kind = ExerciseSetKind.Repetitions
        Repetitions = repetitions
        Weight = 0.0<kg>
        Distance = 0.0<m>
        Duration = TimeSpan.Zero
    }

    static member ofRepetitionsWithWeight repetitions weight : ExerciseSet = {
        Kind = ExerciseSetKind.RepetitionsWithWeight
        Repetitions = repetitions
        Weight = weight
        Distance = 0.0<m>
        Duration = TimeSpan.Zero
    }

    static member ofDuration duration : ExerciseSet = {
        Kind = ExerciseSetKind.Duration
        Repetitions = 0u
        Weight = 0.0<kg>
        Distance = 0.0<m>
        Duration = duration
    }

    static member ofDurationWithWeight duration weight : ExerciseSet = {
        Kind = ExerciseSetKind.DurationWithWeight
        Repetitions = 0u
        Weight = weight
        Distance = 0.0<m>
        Duration = duration
    }

    static member ofDurationWithDistance duration distance : ExerciseSet = {
        Kind = ExerciseSetKind.DurationWithDistance
        Repetitions = 0u
        Weight = 0.0<kg>
        Distance = distance
        Duration = duration
    }
