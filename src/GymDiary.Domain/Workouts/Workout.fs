namespace GymDiary.Domain.Workouts

open System
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users
open GymDiary.Domain.Workouts.Snapshots

/// An exercise completed at a particular time
type Exercise = {
    Definition: ExerciseDefinitionSnapshot
    Sets: ExerciseSet list
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

module Workout =
    let create id routine exercises startedOn completedOn ownerId =
        if exercises |> List.isEmpty then
            failwith "Workout must have at least one exercise"
        elif startedOn > completedOn then
            failwith "Workout cannot be completed before it started"

        {
            Id = id
            Routine = routine
            Exercises = exercises
            StartedOn = startedOn
            CompletedOn = completedOn
            OwnerId = ownerId
        }
