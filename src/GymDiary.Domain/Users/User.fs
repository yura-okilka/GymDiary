namespace GymDiary.Domain.Users

open System
open FSharp.UMX
open GymDiary.Domain.Primitives.SharedTypes

type Gender =
    | Male
    | Female
    | Other

[<Measure>]
type userId

type UserId = Guid<userId>

type User = {
    Id: UserId
    Email: EmailAddress
    FirstName: String50
    LastName: String50
    PhoneNumber: PhoneNumber option
    DateOfBirth: DateOnly option
    Gender: Gender option
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
}

module User =
    let create id email firstName lastName phoneNumber dateOfBirth gender utcNow = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        PhoneNumber = phoneNumber
        DateOfBirth = dateOfBirth
        Gender = gender
        CreatedOnUtc = utcNow
        UpdatedOnUtc = utcNow
    }

    let update firstName lastName phoneNumber dateOfBirth gender user utcNow = {
        user with
            FirstName = firstName
            LastName = lastName
            PhoneNumber = phoneNumber
            DateOfBirth = dateOfBirth
            Gender = gender
            UpdatedOnUtc = utcNow
    }
