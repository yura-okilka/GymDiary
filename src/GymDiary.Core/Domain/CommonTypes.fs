namespace GymDiary.Core.Domain.CommonTypes

open System

open FSharp.Data.UnitSystems.SI.UnitSymbols

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

type Sex =
    | Male
    | Female
    | Other

/// Useful functions for constrained types
module ConstrainedType =

    open System.Text.RegularExpressions

    /// Create a constrained string using the constructor provided
    let createString (fieldName: string) (ctor: string -> 'a) (minLength: int, maxLength: int) (value: string) =
        if value = null then
            Error(ValueNull fieldName)
        elif value.Length < minLength then
            Error(LengthLessThanLimit(fieldName, minLength.ToString()))
        elif value.Length > maxLength then
            Error(LengthGreaterThanLimit(fieldName, maxLength.ToString()))
        else
            Ok(ctor value)

    /// Create an optional constrained string using the constructor provided
    let createStringOption (fieldName: string) (ctor: string -> 'a) (maxLength: int) (value: string) =
        if String.IsNullOrEmpty(value) then
            Ok(None)
        elif value.Length > maxLength then
            Error(LengthGreaterThanLimit(fieldName, maxLength.ToString()))
        else
            Ok(Some(ctor value))

    /// Create a constrained integer using the constructor provided
    let createInt (fieldName: string) (ctor: int -> 'a) (minValue: int, maxValue: int) (value: int) =
        if value < minValue then
            Error(ValueLessThanLimit(fieldName, minValue.ToString()))
        elif value > maxValue then
            Error(ValueGreaterThanLimit(fieldName, maxValue.ToString()))
        else
            Ok(ctor value)

    /// Create a constrained decimal<kg> using the constructor provided
    let createDecimalKg
        (fieldName: string)
        (ctor: decimal<kg> -> 'a)
        (minValue: decimal<kg>, maxValue: decimal<kg>)
        (value: decimal<kg>)
        =
        if value < minValue then
            Error(ValueLessThanLimit(fieldName, minValue.ToString()))
        elif value > maxValue then
            Error(ValueGreaterThanLimit(fieldName, maxValue.ToString()))
        else
            Ok(ctor value)

    /// Create a constrained string using the constructor provided
    let createLike (fieldName: string) (ctor: string -> 'a) (pattern: string) (value: string) =
        if String.IsNullOrEmpty(value) then
            Error(ValueNullOrEmpty fieldName)
        elif Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)) then
            Ok(ctor value)
        else
            Error(RegexNotMatched(fieldName))

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

    let value (EmailAddress value) = value

    let create fieldName value =
        let pattern = ".+@.+" // anything separated by an "@"
        ConstrainedType.createLike fieldName EmailAddress pattern value
