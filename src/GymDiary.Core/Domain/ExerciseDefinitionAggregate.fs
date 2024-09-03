module GymDiary.Core.Domain.ExerciseDefinitionAggregate

open System
open GymDiary.Core.Domain.ExerciseCategoryAggregate
open GymDiary.Core.Domain.UserAggregate

/// A template of an exercise
type ExerciseDefinition = {
    Id: Id<ExerciseDefinition>
    CategoryId: Id<ExerciseCategory>
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSets
    CreatedOn: DateTime
    LastModifiedOn: DateTime
    OwnerId: Id<User>
}

type ExerciseDefinitionId = Id<ExerciseDefinition>

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
