namespace GymDiary.Domain.ExerciseDefinitions

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

type ExerciseSetKind =
    | Repetitions
    | RepetitionsWithWeight
    | Duration
    | DurationWithWeight
    | DurationWithDistance

type ExerciseSetData = {
    Repetitions: uint
    Weight: float<kg>
    Distance: float<m>
    Duration: TimeSpan
} with

    static member createFromRepetitions repetitions : ExerciseSetData = {
        Repetitions = repetitions
        Weight = 0.0<kg>
        Distance = 0.0<m>
        Duration = TimeSpan.Zero
    }

    static member createFromRepetitionsWithWeight repetitions weight : ExerciseSetData = {
        Repetitions = repetitions
        Weight = weight
        Distance = 0.0<m>
        Duration = TimeSpan.Zero
    }

    static member createFromDuration duration : ExerciseSetData = {
        Repetitions = 0u
        Weight = 0.0<kg>
        Distance = 0.0<m>
        Duration = duration
    }

    static member createFromDurationWithWeight duration weight : ExerciseSetData = {
        Repetitions = 0u
        Weight = weight
        Distance = 0.0<m>
        Duration = duration
    }

    static member createFromDurationWithDistance duration distance : ExerciseSetData = {
        Repetitions = 0u
        Weight = 0.0<kg>
        Distance = distance
        Duration = duration
    }

type ExerciseSetCollection = {
    Type: ExerciseSetKind
    Items: ExerciseSetData list
}
