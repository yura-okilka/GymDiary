namespace GymDiary.Core.Domain

module Sportsman =

    /// Restores exercise from provided data. Use only for serialization.
    let restoreFrom id email firstName lastName dateOfBirth gender : Sportsman = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }

    let create email firstName lastName dateOfBirth gender : Sportsman = {
        Id = Id.Empty
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }
