module GymDiary.Domain.UnitTests.Workouts.WorkoutTests

open System

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users
open GymDiary.Domain.Workouts
open GymDiary.Domain.Workouts.Snapshots

open Xunit
open FsUnitTyped

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private str s = String50.create s |> value

let private startedOn = DateTime(2026, 6, 7, 10, 0, 0, DateTimeKind.Utc)
let private completedOn = DateTime(2026, 6, 7, 11, 0, 0, DateTimeKind.Utc)

let private routineSnapshot: RoutineSnapshot = {
    Id = EntityId.create<routineId> ()
    Name = str "Push day"
    Goal = None
    Notes = None
    Schedule = Set.empty
}

let private definitionSnapshot: ExerciseDefinitionSnapshot = {
    Id = EntityId.create<exerciseDefinitionId> ()
    Category = { Id = EntityId.create<exerciseCategoryId> (); Name = str "Chest" }
    Name = str "Bench press"
    Notes = None
    RestTime = TimeSpan.Zero
    Sets = RepetitionSets [ PositiveInt.create 10 |> value ]
}

let private exercise start completed : Exercise = {
    Definition = definitionSnapshot
    Sets = RepetitionSets [ PositiveInt.create 10 |> value ]
    StartedOn = start
    CompletedOn = completed
}

let private create exercises startedOn completedOn =
    Workout.create (EntityId.create<workoutId> ()) routineSnapshot exercises startedOn completedOn (EntityId.create<userId> ())

[<Fact>]
let ``create with valid input succeeds`` () =
    create [ exercise startedOn completedOn ] startedOn completedOn
    |> Result.isOk
    |> shouldEqual true

[<Fact>]
let ``create with no exercises fails`` () =
    create [] startedOn completedOn |> shouldEqual (Error WorkoutError.MustHaveAtLeastOneExercise)

[<Fact>]
let ``create completed before started fails`` () =
    create [ exercise startedOn completedOn ] completedOn startedOn
    |> shouldEqual (Error WorkoutError.CompletedBeforeStarted)

[<Fact>]
let ``create with an exercise completed before it started fails`` () =
    create [ exercise completedOn startedOn ] startedOn completedOn
    |> shouldEqual (Error WorkoutError.ExerciseCompletedBeforeStarted)
