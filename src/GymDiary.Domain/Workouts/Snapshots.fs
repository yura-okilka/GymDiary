namespace GymDiary.Domain.Workouts.Snapshots

open System
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

/// A snapshot of exercise category at a particular time
type ExerciseCategorySnapshot = {
    Id: Id<ExerciseCategory>
    Name: String50
    OwnerId: Id<User>
}

/// A snapshot of exercise definition at a particular time
type ExerciseDefinitionSnapshot = {
    Id: Id<ExerciseDefinition>
    Category: ExerciseCategorySnapshot
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSet list
    OwnerId: Id<User>
}

/// A snapshot of routine at a particular time
type RoutineSnapshot = {
    Id: Id<Routine>
    Name: String50
    Goal: String200 option
    Notes: String1k option
    Schedule: DayOfWeek Set
    OwnerId: Id<User>
}
