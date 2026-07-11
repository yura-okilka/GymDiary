module GymDiary.Persistence.MongoDB.UnitTests.Mapping.ExerciseSetDtoMapperTests

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Application.Workflows.Validation
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Persistence.MongoDB.Mapping

open Xunit
open FsUnitTyped

let private value =
    function
    | Ok v -> v
    | Error e -> failwith (string e)

let private pos n = PositiveInt.create n |> value

let private mapper =
    ExerciseSetDtoMapper() :> IDocumentMapper<ExerciseSetGroup, ExerciseSetDto list>

let private roundTrip (sets: ExerciseSetGroup) =
    sets |> mapper.MapFromDomain |> mapper.MapToDomain

let private errorField result =
    match result with
    | Ok _ -> failwith "expected Error"
    | Error(ValidationError(field, _)) -> field

[<Fact>]
let ``round-trips repetition sets`` () =
    let sets = RepetitionSets [ pos 10; pos 8 ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips weighted repetition sets`` () =
    let sets = WeightedRepetitionSets [ (pos 10, 60.0<kg>); (pos 8, 65.0<kg>) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips duration sets`` () =
    let sets = DurationSets [ TimeSpan.FromSeconds 30.0; TimeSpan.FromSeconds 45.0 ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips weighted duration sets`` () =
    let sets = WeightedDurationSets [ (TimeSpan.FromSeconds 30.0, 20.0<kg>) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``round-trips distance duration sets`` () =
    let sets = TimedDistanceSets [ (1000.0<m>, TimeSpan.FromMinutes 5.0) ]
    roundTrip sets |> shouldEqual (Ok sets)

[<Fact>]
let ``MapToDomain of an empty list fails`` () =
    mapper.MapToDomain [] |> errorField |> shouldEqual "Sets"

[<Fact>]
let ``MapToDomain of mixed kinds fails`` () =
    let mixed =
        (RepetitionSets [ pos 10 ] |> mapper.MapFromDomain)
        @ (DurationSets [ TimeSpan.FromSeconds 30.0 ] |> mapper.MapFromDomain)

    mapper.MapToDomain mixed |> errorField |> shouldEqual "Sets"

[<Fact>]
let ``MapToDomain of non-positive repetitions fails`` () =
    let zeroReps = [
        {
            Kind = ExerciseSetKindDto.Repetitions
            Repetitions = 0u
            Weight = 0.0
            Distance = 0.0
            Duration = TimeSpan.Zero
        }
    ]

    mapper.MapToDomain zeroReps |> errorField |> shouldEqual "Repetitions"
