namespace GymDiary.Core.Domain

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols
open GymDiary.Core.Domain.ExerciseCategoryAggregate
open GymDiary.Core.Domain.UserAggregate

module ExerciseDefinitionAggregate =

    type ExerciseSetType =
        | Repetitions
        | RepetitionsWithWeight
        | Duration
        | DurationWithWeight
        | DurationWithDistance

    type ExerciseSetData = {
        SequenceNumber: PositiveInt
        Repetitions: uint
        Weight: float<kg>
        Distance: float<m>
        Duration: TimeSpan
    }

    type ExerciseSets = {
        Type: ExerciseSetType
        Items: ExerciseSetData list
    }

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
