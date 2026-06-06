namespace GymDiary.Domain.ExerciseDefinitions

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols
open GymDiary.Domain.Primitives.SharedTypes

/// The sets of a single exercise. The kind is lifted to the whole collection, so a mix of
/// (say) repetition and duration sets is unrepresentable — every set shares one kind by construction.
type ExerciseSets =
    | RepetitionSets of PositiveInt list
    | WeightedRepetitionSets of (PositiveInt * float<kg>) list
    | DurationSets of TimeSpan list
    | WeightedDurationSets of (TimeSpan * float<kg>) list
    | DistanceDurationSets of (TimeSpan * float<m>) list
