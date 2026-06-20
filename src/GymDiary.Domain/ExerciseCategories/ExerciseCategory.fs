namespace GymDiary.Domain.ExerciseCategories

open System
open FSharp.UMX
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

[<Measure>]
type exerciseCategoryId

type ExerciseCategoryId = Guid<exerciseCategoryId>

type ExerciseCategory = {
    Id: ExerciseCategoryId
    Name: String50
    OwnerId: UserId
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
}

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
