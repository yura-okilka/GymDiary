namespace GymDiary.Infrastructure.Persistence.Documents

open System

type GenderDtoV2 =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type UserDocumentV2 = {
    Id: string
    Email: string
    FirstName: string
    LastName: string
    DateOfBirth: DateTime option
    Gender: GenderDtoV2 option
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocument with
        member d.Id = d.Id
