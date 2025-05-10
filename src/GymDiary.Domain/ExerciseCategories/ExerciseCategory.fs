namespace GymDiary.Domain.ExerciseCategories

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseCategory = {
    Id: Id<ExerciseCategory>
    Name: String50
    OwnerId: Id<User>
}

type ExerciseCategoryId = Id<ExerciseCategory>

module ExerciseCategory =
    let create id name ownerId = {
        Id = id
        Name = name
        OwnerId = ownerId
    }

    let rename name category = { category with Name = name }
