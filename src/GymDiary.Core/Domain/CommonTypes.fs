namespace GymDiary.Core.Domain

open System

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

module String50 =

    let create fieldName value =
        ConstrainedType.createString fieldName String50 (1, 50) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String50 50 value

    let value (String50 value) = value

module String200 =

    let create fieldName value =
        ConstrainedType.createString fieldName String200 (1, 200) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String200 200 value

    let value (String200 value) = value

module String1k =

    let create fieldName value =
        ConstrainedType.createString fieldName String1k (1, 1000) value

    let createOption fieldName value =
        ConstrainedType.createStringOption fieldName String1k 1000 value

    let value (String1k value) = value

module PositiveInt =

    let create fieldName number =
        ConstrainedType.createInt fieldName PositiveInt (1, Int32.MaxValue) number

    let value (PositiveInt number) = number

module EmailAddress =

    open System.ComponentModel.DataAnnotations

    let create (fieldName: string) (value: string) =
        let attribute = new EmailAddressAttribute()

        if attribute.IsValid(value) then
            EmailAddress value |> Ok
        else
            ValidationError(fieldName, InvalidEmailAddress) |> Error

    let value (EmailAddress value) = value

module PhoneNumber =

    open System.ComponentModel.DataAnnotations

    let create (fieldName: string) (value: string) =
        let attribute = new PhoneAttribute()

        if attribute.IsValid(value) then
            PhoneNumber value |> Ok
        else
            ValidationError(fieldName, InvalidPhoneNumber) |> Error

    let value (PhoneNumber value) = value
