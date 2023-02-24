namespace GymDiary.Core.Domain

module Sportsman =

    let create id email firstName lastName dateOfBirth gender : Sportsman = {
        Id = id
        Email = email
        FirstName = firstName
        LastName = lastName
        DateOfBirth = dateOfBirth
        Gender = gender
    }
