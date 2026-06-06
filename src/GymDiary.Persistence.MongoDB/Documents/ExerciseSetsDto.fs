namespace GymDiary.Persistence.MongoDB.Documents

open System

type ExerciseSetKindDto =
    | Repetitions = 1
    | RepetitionsWithWeight = 2
    | Duration = 3
    | DurationWithWeight = 4
    | DurationWithDistance = 5

[<CLIMutable>]
type ExerciseSetDto = {
    Kind: ExerciseSetKindDto
    Repetitions: uint
    Weight: float
    Distance: float
    Duration: TimeSpan
}
