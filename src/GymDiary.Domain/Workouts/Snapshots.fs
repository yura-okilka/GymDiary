namespace GymDiary.Domain.Workouts.Snapshots

open System
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines

/// A snapshot of exercise category at a particular time
type ExerciseCategorySnapshot = {
    Id: ExerciseCategoryId
    Name: String50
}

/// A snapshot of exercise definition at a particular time
type ExerciseDefinitionSnapshot = {
    Id: ExerciseDefinitionId
    Category: ExerciseCategorySnapshot
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSets
}

/// A snapshot of routine at a particular time
type RoutineSnapshot = {
    Id: RoutineId
    Name: String50
    Goal: String200 option
    Notes: String1k option
    Schedule: DayOfWeek Set
}
