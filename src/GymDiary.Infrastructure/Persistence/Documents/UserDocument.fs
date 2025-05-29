namespace GymDiary.Infrastructure.Persistence.Documents

open System

type GenderDto =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type UserDocument = {
    Id: string
    Email: string
    FirstName: string
    LastName: string
    DateOfBirth: DateTime option
    Gender: GenderDto option
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
} with

    interface IDocument with
        member d.Id = d.Id
