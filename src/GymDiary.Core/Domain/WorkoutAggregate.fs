module GymDiary.Core.Domain.WorkoutAggregate

open System
open GymDiary.Core.Domain.ExerciseCategoryAggregate
open GymDiary.Core.Domain.ExerciseDefinitionAggregate
open GymDiary.Core.Domain.RoutineAggregate
open GymDiary.Core.Domain.UserAggregate

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
    Sets: ExerciseSets
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

/// An exercise completed at a particular time
type Exercise = {
    Definition: ExerciseDefinitionSnapshot
    Sets: ExerciseSets
    StartedOn: DateTime
    CompletedOn: DateTime
}

/// A workout completed at a particular time
type Workout = {
    Id: Id<Workout>
    Routine: RoutineSnapshot
    Exercises: Exercise list
    StartedOn: DateTime
    CompletedOn: DateTime
    OwnerId: Id<User>
}

type WorkoutId = Id<Workout>

let create id routine exercises startedOn completedOn ownerId : Workout = {
    Id = id
    Routine = routine
    Exercises = exercises
    StartedOn = startedOn
    CompletedOn = completedOn
    OwnerId = ownerId
}
