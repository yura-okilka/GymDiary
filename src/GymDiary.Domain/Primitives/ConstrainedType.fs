module GymDiary.Domain.Primitives.ConstrainedType

/// Create a constrained string
let createString (ctor: string -> 'a) (minLength: int, maxLength: int) (value: string) =
    if value.Length < minLength || value.Length > maxLength then
        Error $"Value must be between {minLength} and {maxLength} characters"
    else
        Ok(ctor value)

/// Create a constrained string if it is not null
let createStringOption (ctor: string -> 'a) (minLength: int, maxLength: int) (value: string | null) =
    match value with
    | null -> Ok None
    | str -> createString ctor (minLength, maxLength) str |> Result.map Some

/// Create a constrained integer
let createInt (ctor: int -> 'a) (minValue: int, maxValue: int) (value: int) =
    if value < minValue || value > maxValue then
        Error $"Value must be between {minValue} and {maxValue}"
    else
        Ok(ctor value)
