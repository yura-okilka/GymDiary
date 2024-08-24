module GymDiary.Core.Domain.ExerciseCategoryAggregate

open GymDiary.Core.Domain.UserAggregate

type ExerciseCategory = {
    Id: Id<ExerciseCategory>
    Name: String50
    OwnerId: Id<User>
}

type ExerciseCategoryId = Id<ExerciseCategory>

let create id name ownerId : ExerciseCategory = {
    Id = id
    Name = name
    OwnerId = ownerId
}

let rename (name: String50) (category: ExerciseCategory) = { category with Name = name }
