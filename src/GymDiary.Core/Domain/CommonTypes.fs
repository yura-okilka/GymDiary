namespace GymDiary.Core.Domain.CommonTypes

open System

open GymDiary.Core.Domain.Errors

/// Constrained to be 50 chars or less, not null
type String50 = private String50 of string

/// Constrained to be 200 chars or less, not null
type String200 = private String200 of string

/// Constrained to be 1000 chars or less, not null
type String1k = private String1k of string

/// Constrained to be a non-zero positive natural number
type PositiveInt = private PositiveInt of int

/// Constrained to be a valid email address
type EmailAddress = private EmailAddress of string

/// Constrained to be a valid phone number
type PhoneNumber = private PhoneNumber of string

type Sex =
    | Male
    | Female
    | Other

/// Useful functions for constrained types
module ConstrainedType =

    open System.Text.RegularExpressions

    open FSharp.Data.UnitSystems.SI.UnitSymbols

    /// Create a constrained string using the constructor provided
    let createString (fieldName: string) (ctor: string -> 'a) (minLength: int, maxLength: int) (value: string) =
        if value = null then
            ValidationError(fieldName, ValueNull) |> Error
        elif value.Length < minLength then
            ValidationError(fieldName, LengthLessThanLimit(minLength.ToString())) |> Error
        elif value.Length > maxLength then
            ValidationError(fieldName, LengthGreaterThanLimit(maxLength.ToString())) |> Error
        else
            ctor value |> Ok

    /// Create an optional constrained string using the constructor provided
    let createStringOption (fieldName: string) (ctor: string -> 'a) (maxLength: int) (value: string) =
        if String.IsNullOrEmpty(value) then
            None |> Ok
        elif value.Length > maxLength then
            ValidationError(fieldName, LengthGreaterThanLimit(maxLength.ToString())) |> Error
        else
            ctor value |> Some |> Ok

    /// Create a constrained integer using the constructor provided
    let createInt (fieldName: string) (ctor: int -> 'a) (minValue: int, maxValue: int) (value: int) =
        if value < minValue then
            ValidationError(fieldName, ValueLessThanLimit(minValue.ToString())) |> Error
        elif value > maxValue then
            ValidationError(fieldName, ValueGreaterThanLimit(maxValue.ToString())) |> Error
        else
            ctor value |> Ok

    /// Create a constrained decimal<kg> using the constructor provided
    let createDecimalKg
        (fieldName: string)
        (ctor: decimal<kg> -> 'a)
        (minValue: decimal<kg>, maxValue: decimal<kg>)
        (value: decimal<kg>)
        =
        if value < minValue then
            ValidationError(fieldName, ValueLessThanLimit(minValue.ToString())) |> Error
        elif value > maxValue then
            ValidationError(fieldName, ValueGreaterThanLimit(maxValue.ToString())) |> Error
        else
            ctor value |> Ok

    /// Create a constrained string using the constructor provided
    let createLike (fieldName: string) (ctor: string -> 'a) (pattern: string) (value: string) =
        if String.IsNullOrEmpty(value) then
            ValidationError(fieldName, ValueNullOrEmpty) |> Error
        elif Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)) then
            ctor value |> Ok
        else
            ValidationError(fieldName, PatternNotMatched) |> Error

module String50 =

    let value (String50 value) = value

    let create fieldName value =
        ConstrainedType.createString fieldName String50 (1, 50) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String50 50 value

module String200 =

    let value (String200 value) = value

    let create fieldName value =
        ConstrainedType.createString fieldName String200 (1, 200) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String200 200 value

module String1k =

    let value (String1k value) = value

    let create fieldName value =
        ConstrainedType.createString fieldName String1k (1, 1000) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String1k 1000 value

module PositiveInt =

    let value (PositiveInt number) = number

    let create fieldName number =
        ConstrainedType.createInt fieldName PositiveInt (1, Int32.MaxValue) number

module EmailAddress =

    open System.ComponentModel.DataAnnotations

    let value (EmailAddress value) = value

    let create (fieldName: string) (value: string) =
        let attribute = new EmailAddressAttribute()

        if attribute.IsValid(value) then
            EmailAddress value |> Ok
        else
            ValidationError(fieldName, InvalidEmailAddress) |> Error

module PhoneNumber =

    open System.ComponentModel.DataAnnotations

    let value (PhoneNumber value) = value

    let create (fieldName: string) (value: string) =
        let attribute = new PhoneAttribute()

        if attribute.IsValid(value) then
            PhoneNumber value |> Ok
        else
            ValidationError(fieldName, InvalidPhoneNumber) |> Error
