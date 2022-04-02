namespace GymDiary.Core.Domain.Errors

type ValidationError =
    | ValueNullOrEmpty of field: string
    | ValueLessThanLimit of field: string * limit: string
    | ValueGreaterThanLimit of field: string * limit: string
    | LengthGreaterThanLimit of field: string * limit: string
    | InvalidValue of field: string * value: string
    | RegexNotMatched of field: string

    static member toString (error: ValidationError) =
        match error with
        | ValueNullOrEmpty field -> $"'%s{field}' must not be null or empty."
        | ValueLessThanLimit (field, limit) -> $"'%s{field}' must be greater than or equal to %s{limit}."
        | ValueGreaterThanLimit (field, limit) -> $"'%s{field}' must be less than or equal to %s{limit}."
        | LengthGreaterThanLimit (field, limit) -> $"The length of '%s{field}' must be %s{limit} characters or fewer."
        | InvalidValue (field, value) -> $"'%s{field}' cannot contain '%s{value}'."
        | RegexNotMatched field -> $"'%s{field}' is not in the correct format."

type PersistenceError =
    | Validation of error: ValidationError
    | Database of ex: exn
    | Other of ex: exn
