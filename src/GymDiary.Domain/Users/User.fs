namespace GymDiary.Domain.Users

open System
open GymDiary.Domain.Primitives.SharedTypes

type Gender =
    | Male
    | Female
    | Other

type User = {
    Id: Id<User>
    Email: EmailAddress
    FirstName: String50
    LastName: String50
    DateOfBirth: DateOnly option
    Gender: Gender option
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
}

type UserId = Id<User>

module User =
    let create id email firstName lastName dateOfBirth gender utcNow = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
        CreatedOnUtc = utcNow
        UpdatedOnUtc = utcNow
    }

    let update firstName lastName dateOfBirth gender user = {
        user with
            FirstName = firstName
            LastName = lastName
            DateOfBirth = dateOfBirth
            Gender = gender
    }
