namespace GymDiary.Infrastructure.Persistence.Documents

open System

[<CLIMutable>]
type ExerciseCategorySnapshotDto = { Id: string; Name: string }

[<CLIMutable>]
type ExerciseDefinitionSnapshotDto = {
    Id: string
    Category: ExerciseCategorySnapshotDto
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetDto list
}

[<CLIMutable>]
type RoutineSnapshotDto = {
    Id: string
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek Set
}

[<CLIMutable>]
type ExerciseDto = {
    Definition: ExerciseDefinitionSnapshotDto
    Sets: ExerciseSetDto list
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
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
