namespace GymDiary.Core.Domain

module Exercise =

    /// Restores exercise from provided data. Use only for serialization.
    let restoreFrom id categoryId name notes restTime sets createdOn lastModifiedOn ownerId : Exercise = { // TODO: restoreFrom
        Id = id
        CategoryId = categoryId
        Name = name
        Notes = notes
        RestTime = restTime
        Sets = sets
        CreatedOn = createdOn
        LastModifiedOn = lastModifiedOn
        OwnerId = ownerId
    }

    let create categoryId ownerId name notes restTime sets utcNow : Exercise = {
        Id = Id.Empty
        CategoryId = categoryId
        OwnerId = ownerId
        Name = name
        Notes = notes
        RestTime = restTime
        Sets = sets // TODO: check order nums
        CreatedOn = utcNow
        LastModifiedOn = utcNow
    }
