namespace GymDiary.Infrastructure.Persistence.Documents

open System

[<CLIMutable>]
type ExerciseCategorySnapshotDto = {
    Id: string
    Name: string
    OwnerId: string
}

[<CLIMutable>]
type ExerciseDefinitionSnapshotDto = {
    Id: string
    Category: ExerciseCategorySnapshotDto
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetsDto
    OwnerId: string
}

[<CLIMutable>]
type RoutineSnapshotDto = {
    Id: string
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek Set
    OwnerId: string
}

[<CLIMutable>]
type ExerciseDto = {
    Definition: ExerciseDefinitionSnapshotDto
    Sets: ExerciseSetsDto
    StartedOn: DateTime
    CompletedOn: DateTime
}

[<CLIMutable>]
type WorkoutDocument = {
    Id: string
    Routine: RoutineSnapshotDto
    Exercises: ExerciseDto list
    StartedOn: DateTime
    CompletedOn: DateTime
    OwnerId: string
}
