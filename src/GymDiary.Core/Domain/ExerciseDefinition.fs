namespace GymDiary.Core.Domain

module ExerciseDefinition =

    /// Restores exercise from provided data. Use only for serialization.
    let restoreFrom id categoryId name notes restTime sets createdOn lastModifiedOn ownerId : ExerciseDefinition = {
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

    let create id categoryId ownerId name notes restTime sets utcNow : ExerciseDefinition = {
        Id = id
        CategoryId = categoryId
        OwnerId = ownerId
        Name = name
        Notes = notes
        RestTime = restTime
        Sets = sets // TODO: check order nums
        CreatedOn = utcNow
        LastModifiedOn = utcNow
    }
