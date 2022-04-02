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
    | DtoConversion of dto: string * error: ValidationError
    | Database of operation: string * ex: exn
    | Other of operation: string * ex: exn

    static member toString (error: PersistenceError) =
        match error with
        | DtoConversion (dto, error) -> $"Failed to convert '%s{dto}': %s{ValidationError.toString error}"
        | Database (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
        | Other (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
