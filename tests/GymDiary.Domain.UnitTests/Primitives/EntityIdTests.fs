module GymDiary.Domain.UnitTests.Primitives.EntityIdTests

open System
open FSharp.UMX
open GymDiary.Domain.Primitives.SharedTypes

open Xunit
open FsUnitTyped

[<Measure>]
type testId

[<Fact>]
let ``create produces a version 7 GUID`` () =
    let id = EntityId.create<testId> ()
    (UMX.untag id).Version |> shouldEqual 7

[<Fact>]
let ``create produces unique ids`` () =
    let a = EntityId.create<testId> ()
    let b = EntityId.create<testId> ()
    a |> shouldNotEqual b

[<Fact>]
let ``parse accepts a valid GUID`` () =
    let raw = Guid.NewGuid()
    let parsed: Result<Guid<testId>, string> = EntityId.parse (string raw)
    parsed |> shouldEqual (Ok(UMX.tag raw))

[<Fact>]
let ``parse rejects a non-GUID string`` () =
    let parsed: Result<Guid<testId>, string> = EntityId.parse "not-a-guid"
    parsed |> Result.isError |> shouldEqual true

[<Fact>]
let ``toString round-trips through parse`` () =
    let id = EntityId.create<testId> ()
    EntityId.parse (string id) |> shouldEqual (Ok id)
