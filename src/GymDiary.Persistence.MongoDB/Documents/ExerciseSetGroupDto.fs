namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type WeightedRepetitionDto = { Repetitions: int; Weight: float }

[<CLIMutable>]
type WeightedDurationDto = { Duration: TimeSpan; Weight: float }

[<CLIMutable>]
type TimedDistanceDto = { Distance: float; Duration: TimeSpan }

/// Persistence mirror of the domain ExerciseSetGroup. Plain persistence types (units of measure and
/// UMX tags erase at runtime) and a discriminated union so the stored kind is encoded by the case,
/// making a mixed-kind group unrepresentable — the same guarantee the domain type gives.
type ExerciseSetGroupDto =
    | RepetitionSets of repetitions: int list
    | WeightedRepetitionSets of sets: WeightedRepetitionDto list
    | DurationSets of durations: TimeSpan list
    | WeightedDurationSets of sets: WeightedDurationDto list
    | TimedDistanceSets of sets: TimedDistanceDto list
