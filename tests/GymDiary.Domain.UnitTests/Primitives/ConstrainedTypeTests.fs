module GymDiary.Domain.UnitTests.Primitives.ConstrainedTypeTests

open GymDiary.Domain.Primitives
open Xunit
open FsUnitTyped

type TestString10 = TestString10 of string
type TestInt1To10 = TestInt1To10 of int

[<Fact>]
let ``createString from valid value succeeds`` () =
    ConstrainedType.createString TestString10 (1, 10) "valid name"
    |> shouldEqual (Ok(TestString10 "valid name"))

[<Theory>]
[<InlineData("")>]
[<InlineData("more than 10 chars")>]
let ``createString from invalid value returns error`` (value: string) =
    ConstrainedType.createString TestString10 (1, 10) value
    |> shouldEqual (Error "Value must be between 1 and 10 characters")

[<Fact>]
let ``createStringOption from some value succeeds`` () =
    ConstrainedType.createStringOption TestString10 (1, 10) "valid name"
    |> shouldEqual (Ok(Some(TestString10 "valid name")))

[<Fact>]
let ``createStringOption from null value returns none`` () =
    ConstrainedType.createStringOption TestString10 (1, 10) null |> shouldEqual (Ok None)

[<Theory>]
[<InlineData("")>]
[<InlineData("more than 10 chars")>]
let ``createStringOption from invalid value returns error`` (value: string) =
    ConstrainedType.createStringOption TestString10 (1, 10) value
    |> shouldEqual (Error "Value must be between 1 and 10 characters")

[<Fact>]
let ``createInt from valid value succeeds`` () =
    ConstrainedType.createInt TestInt1To10 (1, 10) 2 |> shouldEqual (Ok(TestInt1To10 2))

[<Theory>]
[<InlineData(0)>]
[<InlineData(11)>]
let ``createInt from invalid value returns error`` (value: int) =
    ConstrainedType.createInt TestInt1To10 (1, 10) value
    |> shouldEqual (Error "Value must be between 1 and 10")
