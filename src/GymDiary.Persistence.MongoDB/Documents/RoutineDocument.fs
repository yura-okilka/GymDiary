namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type RoutineDocument = {
    Id: string
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek Set
    ExerciseIds: string Set
    OwnerId: string
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
