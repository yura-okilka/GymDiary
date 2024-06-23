namespace GymDiary.Core.Domain

module User =

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
