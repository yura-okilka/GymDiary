module GymDiary.Persistence.MongoDB.UnitTests.Serialization.ExerciseSetGroupSerializationTests

open System

open MongoDB.Bson
open MongoDB.Bson.Serialization

open GymDiary.Persistence.MongoDB
open GymDiary.Persistence.MongoDB.Documents

open Xunit
open FsUnitTyped

// Verifies the redesigned DTO through the real production serialization settings (F# DU serializer +
// camelCase convention), not just the domain<->DTO mapper. `register` is idempotent (lazy-guarded).
let private toDoc (value: 'T) : BsonDocument =
    SerializationSettings.register ()
    value.ToBsonDocument()

[<Fact>]
let ``weighted repetition sets serialize to a tagged, named sub-document`` () =
    let doc = toDoc (ExerciseSetGroupDto.WeightedRepetitionSets [ { Repetitions = 10; Weight = 60.0 } ])

    doc.["_t"].AsString |> shouldEqual "WeightedRepetitionSets"

    let first = doc.["sets"].AsBsonArray.[0].AsBsonDocument
    first.["repetitions"].AsInt32 |> shouldEqual 10
    first.["weight"].AsDouble |> shouldEqual 60.0

[<Fact>]
let ``round-trips every case through BSON under production settings`` () =
    let cases =
        [ ExerciseSetGroupDto.RepetitionSets [ 10; 8 ]
          ExerciseSetGroupDto.WeightedRepetitionSets [ { Repetitions = 10; Weight = 60.0 } ]
          ExerciseSetGroupDto.DurationSets [ TimeSpan.FromSeconds 30.0 ]
          ExerciseSetGroupDto.WeightedDurationSets [ { Duration = TimeSpan.FromSeconds 30.0; Weight = 20.0 } ]
          ExerciseSetGroupDto.TimedDistanceSets [ { Distance = 1000.0; Duration = TimeSpan.FromMinutes 5.0 } ] ]

    for value in cases do
        toDoc value |> BsonSerializer.Deserialize<ExerciseSetGroupDto> |> shouldEqual value
