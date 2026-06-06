namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>]
type ExerciseCategoryDocument = {
    Id: string
    Name: string
    OwnerId: string
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocumentWithOwner with
        member d.Id = d.Id
        member d.OwnerId = d.OwnerId
