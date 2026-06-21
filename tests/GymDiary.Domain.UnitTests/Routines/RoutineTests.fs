module GymDiary.Domain.UnitTests.Routines.RoutineTests

open System

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users

open Xunit
open FsUnitTyped

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private exercises count : ExerciseDefinitionId Set =
    List.init count (fun _ -> EntityId.create<exerciseDefinitionId> ()) |> Set.ofList

let private routineCreatedOn (createdOn: DateTime) =
    let name = String50.create "Push day" |> value
    Routine.create (EntityId.create<routineId> ()) name None None Set.empty (exercises 1) (EntityId.create<userId> ()) createdOn
    |> value

[<Fact>]
let ``update preserves CreatedOnUtc and bumps UpdatedOnUtc`` () =
    let createdOn = DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let updatedOn = DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc)

    let routine = routineCreatedOn createdOn
    let name = String50.create "Pull day" |> value

    let updated =
        Routine.update name None None Set.empty (exercises 2) routine updatedOn
        |> value

    updated.CreatedOnUtc |> shouldEqual createdOn
    updated.UpdatedOnUtc |> shouldEqual updatedOn

[<Fact>]
let ``update with no exercises fails`` () =
    let routine = routineCreatedOn (DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))
    let name = String50.create "Pull day" |> value

    Routine.update name None None Set.empty Set.empty routine (DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc))
    |> Result.isError
    |> shouldEqual true
