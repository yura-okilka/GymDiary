namespace GymDiary.Core.Domain.Errors

type DomainError = | ExerciseCategoryAlreadyExists

type ValidationError =
    | ValueNull of field: string
    | ValueNullOrEmpty of field: string
    | ValueLessThanLimit of field: string * limit: string
    | ValueGreaterThanLimit of field: string * limit: string
    | LengthLessThanLimit of field: string * limit: string
    | LengthGreaterThanLimit of field: string * limit: string
    | InvalidValue of field: string * value: string
    | RegexNotMatched of field: string

    static member toString (error: ValidationError) =
        match error with
        | ValueNull field -> $"'%s{field}' must not be null."
        | ValueNullOrEmpty field -> $"'%s{field}' must not be null or empty."
        | ValueLessThanLimit (field, limit) -> $"'%s{field}' must be greater than or equal to %s{limit}."
        | ValueGreaterThanLimit (field, limit) -> $"'%s{field}' must be less than or equal to %s{limit}."
        | LengthLessThanLimit (field, limit) -> $"The length of '%s{field}' must be at least %s{limit} characters."
        | LengthGreaterThanLimit (field, limit) -> $"The length of '%s{field}' must be %s{limit} characters or fewer."
        | InvalidValue (field, value) -> $"'%s{field}' cannot contain '%s{value}'."
        | RegexNotMatched field -> $"'%s{field}' is not in the correct format."

type PersistenceError =
    | NotFound of entity: string
    | DtoConversion of dto: string * error: ValidationError
    | Database of operation: string * ex: exn
    | Other of operation: string * ex: exn

    static member notFound entity = NotFound(entity)
    static member dtoConversion dto error = DtoConversion(dto, error)
    static member database operation ex = Database(operation, ex)
    static member other operation ex = Other(operation, ex)

    static member toString (error: PersistenceError) =
        match error with
        | NotFound entity -> $"%s{entity} is not found."
        | DtoConversion (dto, error) -> $"Failed to convert '%s{dto}': %s{ValidationError.toString error}"
        | Database (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
        | Other (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
