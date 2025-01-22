namespace GymDiary.Domain.ExerciseDefinitions

open System
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseDefinition = {
    Id: Id<ExerciseDefinition>
    CategoryId: Id<ExerciseCategory>
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSetCollection
    OwnerId: Id<User>
} with

    static member create id categoryId name notes restTime sets ownerId : ExerciseDefinition = {
        Id = id
        CategoryId = categoryId
        Name = name
        Notes = notes
        RestTime = restTime
        Sets = sets
        OwnerId = ownerId
    }

    static member update categoryId name notes restTime sets definition : ExerciseDefinition = {
        definition with
            CategoryId = categoryId
            Name = name
            Notes = notes
            RestTime = restTime
            Sets = sets
    }

type ExerciseDefinitionId = Id<ExerciseDefinition>
