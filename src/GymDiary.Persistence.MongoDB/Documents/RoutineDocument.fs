namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type RoutineDocument = {
    Id: Guid
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek Set
    ExerciseIds: Guid Set
    OwnerId: Guid
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
