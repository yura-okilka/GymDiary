namespace GymDiary.Core.Domain

open System
open System.Text.RegularExpressions

open FSharp.Data.UnitSystems.SI.UnitSymbols

/// Useful functions for constrained types
module ConstrainedType =

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

    /// Create a constrained string using the constructor provided
    let createStringLike (fieldName: string) (ctor: string -> 'a) (pattern: string) (value: string) =
        if String.IsNullOrEmpty(value) then
            ValidationError(fieldName, ValueNullOrEmpty) |> Error
        elif Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)) then
            ctor value |> Ok
        else
            ValidationError(fieldName, PatternNotMatched) |> Error

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
