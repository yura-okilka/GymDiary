namespace GymDiary.Domain.ExerciseCategories

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseCategory = {
    Id: Id<ExerciseCategory>
    Name: String50
    OwnerId: Id<User>
} with

    static member create id name ownerId : ExerciseCategory = {
        Id = id
        Name = name
        OwnerId = ownerId
    }

    static member rename name category = { category with Name = name }

type ExerciseCategoryId = Id<ExerciseCategory>
