namespace GymDiary.Domain.Workouts

open System
open FSharp.UMX
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users
open GymDiary.Domain.Workouts.Snapshots

/// An exercise completed at a particular time
type Exercise = {
    Definition: ExerciseDefinitionSnapshot
    Sets: ExerciseSetGroup
    StartedOn: DateTime
    CompletedOn: DateTime
}

[<Measure>]
type workoutId

type WorkoutId = Guid<workoutId>

/// A workout completed at a particular time
type Workout = {
    Id: WorkoutId
    Routine: RoutineSnapshot
    Exercises: Exercise list
    StartedOn: DateTime
    CompletedOn: DateTime
    OwnerId: UserId
}

type WorkoutError =
    | MustHaveAtLeastOneExercise
    | CompletedBeforeStarted
    | ExerciseCompletedBeforeStarted

module Workout =
    let create id routine exercises startedOn completedOn ownerId : Result<Workout, WorkoutError> =
        if List.isEmpty exercises then
            Error MustHaveAtLeastOneExercise
        elif startedOn > completedOn then
            Error CompletedBeforeStarted
        elif exercises |> List.exists (fun (e: Exercise) -> e.StartedOn > e.CompletedOn) then
            Error ExerciseCompletedBeforeStarted
        else
            Ok {
                Id = id
                Routine = routine
                Exercises = exercises
                StartedOn = startedOn
                CompletedOn = completedOn
                OwnerId = ownerId
            }
