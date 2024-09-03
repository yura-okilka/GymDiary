module GymDiary.Core.Domain.UserAggregate

open System

type User = {
    Id: Id<User>
    Email: EmailAddress
    FirstName: String50
    LastName: String50
    DateOfBirth: DateOnly option
    Gender: Gender option
}

type UserId = Id<User>

let create id email firstName lastName dateOfBirth gender : User = {
    Id = id
    Email = email
    FirstName = firstName
    LastName = lastName
    DateOfBirth = dateOfBirth
    Gender = gender
}
