namespace GymDiary.Core.Domain

module Routine =

    let create id name goal notes schedule exercises createdOn lastModifiedOn ownerId : Routine = {
        Id = id
        Name = name
        Goal = goal
        Notes = notes
        Schedule = schedule
        Exercises = exercises
        CreatedOn = createdOn
        LastModifiedOn = lastModifiedOn
        OwnerId = ownerId
    }
