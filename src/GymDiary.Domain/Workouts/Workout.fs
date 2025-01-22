namespace GymDiary.Domain.Workouts

open System
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users
open GymDiary.Domain.Workouts.Snapshots

/// An exercise completed at a particular time
type Exercise = {
    Definition: ExerciseDefinitionSnapshot
    Sets: ExerciseSetCollection
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
} with

    static member create id routine exercises startedOn completedOn ownerId : Workout = {
        Id = id
        Routine = routine
        Exercises = exercises
        StartedOn = startedOn
        CompletedOn = completedOn
        OwnerId = ownerId
    }

type WorkoutId = Id<Workout>
