namespace GymDiary.Core.Domain

open System

module UserAggregate =

    type User = {
        Id: Id<User>
        Email: EmailAddress
        FirstName: String50
        LastName: String50
        DateOfBirth: DateOnly option
        Gender: Gender option
    }

    type UserId = Id<User>

    /// Restores exercise from provided data. Use only for serialization.
    let restoreFrom id email firstName lastName dateOfBirth gender : User = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }

    let create id email firstName lastName dateOfBirth gender : User = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }
