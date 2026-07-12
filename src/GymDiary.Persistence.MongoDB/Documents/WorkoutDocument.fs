namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type ExerciseCategorySnapshotDto = { Id: Guid; Name: string }

[<CLIMutable>]
type ExerciseDefinitionSnapshotDto = {
    Id: Guid
    Category: ExerciseCategorySnapshotDto
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetGroupDto
}

[<CLIMutable>]
type RoutineSnapshotDto = {
    Id: Guid
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek Set
}

[<CLIMutable>]
type ExerciseDto = {
    Definition: ExerciseDefinitionSnapshotDto
    Sets: ExerciseSetGroupDto
    StartedOn: DateTime
    CompletedOn: DateTime
}

[<CLIMutable>]
type WorkoutDocument = {
    Id: Guid
    Routine: RoutineSnapshotDto
    Exercises: ExerciseDto list
    StartedOn: DateTime
    CompletedOn: DateTime
    OwnerId: Guid
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
