module GymDiary.Core.Domain.CommonTypes

open System
open FSharp.Data.UnitSystems.SI.UnitSymbols

/// Constrained to be 50 chars or less, not null
type String50 = private String50 of string

/// Constrained to be 200 chars or less, not null
type String200 = private String200 of string

/// Constrained to be 1000 chars or less, not null
type String1k = private String1k of string

/// Constrained to be a non-zero positive natural number
type PositiveInt = private PositiveInt of int

/// Constrained to be a decimal kilogram between 0.1 and 1000.00
type EquipmentWeightKg = private EquipmentWeightKg of decimal<kg>

/// Constrained to be a valid email address
type EmailAddress = private EmailAddress of string

type Sex = Male | Female | Other

/// Useful functions for constrained types
module ConstrainedType =

    open System.Text.RegularExpressions

    /// Create a constrained string using the constructor provided
    /// Return Error if input is null, empty, or length > maxLen
    let createString fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s must not be null or empty" fieldName
            Error msg
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen
            Error msg 
        else
            Ok (ctor str)

    /// Create an optional constrained string using the constructor provided
    /// Return None if input is null, empty. 
    /// Return error if length > maxLen
    /// Return Some if the input is valid
    let createStringOption fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen
            Error msg 
        else
            Ok (ctor str |> Some)

    /// Create a constrained integer using the constructor provided
    /// Return Error if input is less than minVal or more than maxVal
    let createInt fieldName ctor minVal maxVal i =
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %i" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %i" fieldName maxVal
            Error msg
        else
            Ok (ctor i)

    /// Create a constrained decimal<kg> using the constructor provided
    /// Return Error if input is less than minVal or more than maxVal
    let createDecimalKg fieldName ctor minVal maxVal i =
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %M" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %M" fieldName maxVal
            Error msg
        else
            Ok (ctor i)

    /// Create a constrained string using the constructor provided
    /// Return Error if input is null. empty, or does not match the regex pattern
    let createLike fieldName ctor pattern str = 
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s: Must not be null or empty" fieldName
            Error msg
        elif Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)) then
            Ok (ctor str)
        else
            let msg = sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern
            Error msg

module String50 =

    let value (String50 str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName String50 50 str

    let createOption fieldName str = 
        ConstrainedType.createStringOption fieldName String50 50 str

module String200 =

    let value (String200 str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName String200 200 str

    let createOption fieldName str = 
        ConstrainedType.createStringOption fieldName String200 200 str

module String1k =

    let value (String1k str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName String1k 1000 str

    let createOption fieldName str = 
        ConstrainedType.createStringOption fieldName String1k 1000 str

module PositiveInt =

    let value (PositiveInt num) = num

    let create fieldName num = 
        ConstrainedType.createInt fieldName PositiveInt 1 Int32.MaxValue num

module EquipmentWeightKg =

    let value (EquipmentWeightKg v) = v

    /// Create an EquipmentWeightKg from a decimal<kg>.
    /// Return Error if input is not a decimal<kg> between 0.1 and 1000.00
    let create fieldName v = 
        ConstrainedType.createDecimalKg fieldName EquipmentWeightKg 0.1M<kg> 1000M<kg> v

module EmailAddress =

    let value (EmailAddress str) = str

    /// Create an EmailAddress from a string
    /// Return Error if input is null, empty, or doesn't have an "@" in it
    let create fieldName str = 
        let pattern = ".+@.+" // anything separated by an "@"
        ConstrainedType.createLike fieldName EmailAddress pattern str
