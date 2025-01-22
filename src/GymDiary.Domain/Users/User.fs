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
} with

    static member create id email firstName lastName dateOfBirth gender : User = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }

    static member update firstName lastName dateOfBirth gender user : User = {
        user with
            FirstName = firstName
            LastName = lastName
            DateOfBirth = dateOfBirth
            Gender = gender
    }

type UserId = Id<User>
