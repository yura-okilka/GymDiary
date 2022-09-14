namespace GymDiary.Core.Domain

module ExerciseCategory =

    let create id name ownerId : ExerciseCategory =
        {
            Id = id
            Name = name
            OwnerId = ownerId
        }

    let rename (name: String50) (category: ExerciseCategory) = { category with Name = name }
