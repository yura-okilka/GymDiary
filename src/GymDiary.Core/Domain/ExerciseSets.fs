namespace GymDiary.Core.Domain

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

type ExerciseSetType = // Kind?
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
