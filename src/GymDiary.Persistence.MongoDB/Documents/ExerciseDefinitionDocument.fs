namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type ExerciseDefinitionDocument = {
    Id: string
    CategoryId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetDto list
    OwnerId: string
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
