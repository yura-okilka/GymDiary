namespace GymDiary.Core.Domain

module Exercise =

    let create id categoryId name notes restTime sets createdOn lastModifiedOn ownerId : Exercise =
        {
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
