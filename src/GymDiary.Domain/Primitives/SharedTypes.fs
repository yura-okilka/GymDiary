module GymDiary.Domain.Primitives.SharedTypes

open System
open System.ComponentModel.DataAnnotations
open FSharp.UMX

/// Generation and parsing of strongly typed entity ids: UMX-tagged, version-7 GUIDs.
[<RequireQualifiedAccess>]
module EntityId =
    /// A new time-ordered (version 7) id tagged with the entity's measure.
    let inline create<[<Measure>] 'm> () : Guid<'m> = UMX.tag (Guid.CreateVersion7())

    /// Parses a string into a tagged id, rejecting anything that is not a valid GUID.
    let inline parse<[<Measure>] 'm> (value: string) : Result<Guid<'m>, string> =
        match Guid.TryParse value with
        | true, guid -> Ok(UMX.tag guid)
        | false, _ -> Error "Id must be a valid GUID"

    /// Renders a tagged id as its canonical 36-character string form.
    let inline toString (id: Guid<'m>) : string = (UMX.untag id).ToString()

/// Constrained to be 50 chars or fewer
[<Struct>]
type String50 =
    private
    | String50 of string

    member s.Value = let (String50 value) = s in value
    static member create value = ConstrainedType.createString String50 (1, 50) value

/// Constrained to be 200 chars or fewer
[<Struct>]
type String200 =
    private
    | String200 of string

    member s.Value = let (String200 value) = s in value
    static member create value = ConstrainedType.createString String200 (1, 200) value
    static member createOption value = ConstrainedType.createStringOption String200 (1, 200) value

/// Constrained to be 1000 chars or fewer
[<Struct>]
type String1k =
    private
    | String1k of string

    member s.Value = let (String1k value) = s in value
    static member create value = ConstrainedType.createString String1k (1, 1000) value
    static member createOption value = ConstrainedType.createStringOption String1k (1, 1000) value

/// Constrained to be a non-zero positive natural number
[<Struct>]
type PositiveInt =
    private
    | PositiveInt of int

    member i.Value = let (PositiveInt value) = i in value

    static member create value =
        ConstrainedType.createInt PositiveInt (1, Int32.MaxValue) value

/// Constrained to be a valid email address
[<Struct>]
type EmailAddress =
    private
    | EmailAddress of string

    member a.Value = let (EmailAddress value) = a in value

    static member create(value: string) =
        if EmailAddressAttribute().IsValid(value) then
            Ok(EmailAddress value)
        else
            Error "Value is not a valid email address"

/// Constrained to be a valid phone number
[<Struct>]
type PhoneNumber =
    private
    | PhoneNumber of string

    member n.Value = let (PhoneNumber value) = n in value

    static member create(value: string) =
        if PhoneAttribute().IsValid(value) then
            Ok(PhoneNumber value)
        else
            Error "Value is not a valid phone number"
