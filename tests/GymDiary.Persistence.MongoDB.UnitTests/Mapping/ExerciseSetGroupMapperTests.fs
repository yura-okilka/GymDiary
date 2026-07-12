module GymDiary.Persistence.MongoDB.UnitTests.Mapping.ExerciseSetGroupMapperTests

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Application.Validation
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping

open Xunit
open FsUnitTyped

// Domain and DTO unions share case names, so cases are qualified by their type throughout.

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private pos n = PositiveInt.create n |> value

let private mapper =
    ExerciseSetGroupMapper() :> IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto>

let private roundTrip (sets: ExerciseSetGroup) =
    sets |> mapper.MapFromDomain |> mapper.MapToDomain

let private errorField result =
    match result with
    | Ok _ -> failwith "expected Error"
    | Error(ValidationError(field, _)) -> field

[<Fact>]
let ``round-trips repetition sets`` () =
    let sets = ExerciseSetGroup.RepetitionSets [ pos 10; pos 8 ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips weighted repetition sets`` () =
    let sets = ExerciseSetGroup.WeightedRepetitionSets [ (pos 10, 60.0<kg>); (pos 8, 65.0<kg>) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips duration sets`` () =
    let sets = ExerciseSetGroup.DurationSets [ TimeSpan.FromSeconds 30.0; TimeSpan.FromSeconds 45.0 ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips weighted duration sets`` () =
    let sets = ExerciseSetGroup.WeightedDurationSets [ (TimeSpan.FromSeconds 30.0, 20.0<kg>) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips distance duration sets`` () =
    let sets = ExerciseSetGroup.TimedDistanceSets [ (1000.0<m>, TimeSpan.FromMinutes 5.0) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips an empty group`` () =
    let sets = ExerciseSetGroup.RepetitionSets []
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``MapToDomain of non-positive repetitions fails`` () =
    mapper.MapToDomain(ExerciseSetGroupDto.RepetitionSets [ 0 ])
    |> errorField
    |> shouldEqual "repetitions"

[<Fact>]
let ``MapToDomain of non-positive weighted repetitions fails`` () =
    mapper.MapToDomain(ExerciseSetGroupDto.WeightedRepetitionSets [ { Repetitions = 0; Weight = 60.0 } ])
    |> errorField
    |> shouldEqual "Repetitions"
