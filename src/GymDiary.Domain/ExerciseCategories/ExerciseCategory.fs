namespace GymDiary.Domain.ExerciseCategories

open System
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseCategory = {
    Id: Id<ExerciseCategory>
    Name: String50
    OwnerId: Id<User>
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
}

type ExerciseCategoryId = Id<ExerciseCategory>

module ExerciseCategory =
    let create id name ownerId utcNow = {
        Id = id
        Name = name
        OwnerId = ownerId
        CreatedOnUtc = utcNow
        UpdatedOnUtc = utcNow
    }

    let rename name category utcNow = {
        category with
            Name = name
            UpdatedOnUtc = utcNow
    }
