namespace GymDiary.Core.Domain

type ValueError =
    | ValueNull
    | ValueNullOrEmpty
    | ValueLessThanLimit of limit: string
    | ValueGreaterThanLimit of limit: string
    | LengthLessThanLimit of limit: string
    | LengthGreaterThanLimit of limit: string
    | InvalidValue of value: string
    | InvalidEmailAddress
    | InvalidPhoneNumber
    | PatternNotMatched

type ValidationError =
    | ValidationError of field: string * error: ValueError

    static member toString (ValidationError (field, error)) =
        match error with
        | ValueNull -> $"'%s{field}' must not be null."
        | ValueNullOrEmpty -> $"'%s{field}' must not be null or empty."
        | ValueLessThanLimit limit -> $"'%s{field}' must be greater than or equal to %s{limit}."
        | ValueGreaterThanLimit limit -> $"'%s{field}' must be less than or equal to %s{limit}."
        | LengthLessThanLimit limit -> $"The length of '%s{field}' must be at least %s{limit} character(s)."
        | LengthGreaterThanLimit limit -> $"The length of '%s{field}' must be %s{limit} character(s) or fewer."
        | InvalidValue value -> $"'%s{field}' cannot contain '%s{value}'."
        | InvalidEmailAddress -> $"'%s{field}' is not a valid email address."
        | InvalidPhoneNumber -> $"'%s{field}' is not a valid phone number."
        | PatternNotMatched -> $"'%s{field}' is not in the correct format."
