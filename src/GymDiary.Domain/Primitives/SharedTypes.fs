module GymDiary.Domain.Primitives.SharedTypes

open System
open System.ComponentModel.DataAnnotations

/// Constrained to be a strongly typed id of entity
[<Struct>]
type Id<'T> =
    | Id of string

    member this.Value = let (Id value) = this in value

/// Constrained to be 50 chars or fewer
[<Struct>]
type String50 =
    private
    | String50 of string

    member this.Value = let (String50 value) = this in value
    static member create value = ConstrainedType.createString String50 (1, 50) value

/// Constrained to be 200 chars or fewer
[<Struct>]
type String200 =
    private
    | String200 of string

    member this.Value = let (String200 value) = this in value
    static member create value = ConstrainedType.createString String200 (1, 200) value
    static member createOption value = ConstrainedType.createStringOption String200 (1, 200) value

/// Constrained to be 1000 chars or fewer
[<Struct>]
type String1k =
    private
    | String1k of string

    member this.Value = let (String1k value) = this in value
    static member create value = ConstrainedType.createString String1k (1, 1000) value
    static member createOption value = ConstrainedType.createStringOption String1k (1, 1000) value

/// Constrained to be a non-zero positive natural number
[<Struct>]
type PositiveInt =
    private
    | PositiveInt of int

    member this.Value = let (PositiveInt value) = this in value

    static member create value =
        ConstrainedType.createInt PositiveInt (1, Int32.MaxValue) value

/// Constrained to be a valid email address
[<Struct>]
type EmailAddress =
    private
    | EmailAddress of string

    member this.Value = let (EmailAddress value) = this in value

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

    member this.Value = let (PhoneNumber value) = this in value

    static member create(value: string) =
        if PhoneAttribute().IsValid(value) then
            Ok(PhoneNumber value)
        else
            Error "Value is not a valid phone number"
