module GymDiary.Persistence.MongoDB.UnitTests.Mapping.ExerciseDefinitionDocumentMapperTests

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping

open Xunit
open FsUnitTyped

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private mapper =
    ExerciseDefinitionDocumentMapper(ExerciseSetDtoMapper())
    :> IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument>

[<Fact>]
let ``round-trips an exercise definition`` () =
    let createdOn = DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let name = String50.create "Bench press" |> value
    let notes = String1k.create "Keep elbows tucked" |> value |> Some
    let sets = WeightedRepetitionSets [ (PositiveInt.create 10 |> value, 60.0<kg>) ]

    let definition =
        ExerciseDefinition.create (EntityId.create<exerciseDefinitionId> ()) (EntityId.create<exerciseCategoryId> ()) name notes (TimeSpan.FromMinutes 2.0) sets (EntityId.create<userId> ()) createdOn
        |> value

    definition |> mapper.MapFromDomain |> mapper.MapToDomain |> shouldEqual (Ok definition)

[<Fact>]
let ``round-trips an exercise definition without notes`` () =
    let createdOn = DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let name = String50.create "Plank" |> value
    let sets = DurationSets [ TimeSpan.FromSeconds 60.0 ]

    let definition =
        ExerciseDefinition.create (EntityId.create<exerciseDefinitionId> ()) (EntityId.create<exerciseCategoryId> ()) name None TimeSpan.Zero sets (EntityId.create<userId> ()) createdOn
        |> value

    definition |> mapper.MapFromDomain |> mapper.MapToDomain |> shouldEqual (Ok definition)
