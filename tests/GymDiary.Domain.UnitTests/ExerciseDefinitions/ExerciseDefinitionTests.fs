module GymDiary.Domain.UnitTests.ExerciseDefinitions.ExerciseDefinitionTests

open System

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Users

open Xunit
open FsUnitTyped

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private repetitionSets counts =
    counts |> List.map (fun n -> PositiveInt.create n |> value) |> RepetitionSets

let private definitionCreatedOn (createdOn: DateTime) =
    let name = String50.create "Bench press" |> value
    let sets = repetitionSets [ 10 ]
    ExerciseDefinition.create (EntityId.create<exerciseDefinitionId> ()) (EntityId.create<exerciseCategoryId> ()) name None TimeSpan.Zero sets (EntityId.create<userId> ()) createdOn
    |> value

[<Fact>]
let ``update preserves CreatedOnUtc and bumps UpdatedOnUtc`` () =
    let createdOn = DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let updatedOn = DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc)

    let definition = definitionCreatedOn createdOn
    let name = String50.create "Incline press" |> value
    let sets = repetitionSets [ 8 ]

    let updated =
        ExerciseDefinition.update (EntityId.create<exerciseCategoryId> ()) name None TimeSpan.Zero sets definition updatedOn
        |> value

    updated.CreatedOnUtc |> shouldEqual createdOn
    updated.UpdatedOnUtc |> shouldEqual updatedOn

[<Fact>]
let ``create with no sets fails`` () =
    let name = String50.create "Bench press" |> value

    ExerciseDefinition.create (EntityId.create<exerciseDefinitionId> ()) (EntityId.create<exerciseCategoryId> ()) name None TimeSpan.Zero (RepetitionSets []) (EntityId.create<userId> ()) (DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))
    |> shouldEqual (Error ExerciseDefinitionError.MustHaveAtLeastOneSet)
