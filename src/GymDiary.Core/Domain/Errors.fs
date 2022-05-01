namespace GymDiary.Core.Domain.Errors

type DomainError =
    | ExerciseCategoryWithNameAlreadyExists of name: string
    | ExerciseCategoryNotFound
    | OwnerNotFound

    static member toString (error: DomainError) =
        match error with
        | ExerciseCategoryWithNameAlreadyExists name -> $"Exercise category '%s{name}' already exists."
        | ExerciseCategoryNotFound -> "Exercise category is not found."
        | OwnerNotFound -> "Owner is not found."

type ValidationError =
    | ValueNull of field: string
    | ValueNullOrEmpty of field: string
    | ValueLessThanLimit of field: string * limit: string
    | ValueGreaterThanLimit of field: string * limit: string
    | LengthLessThanLimit of field: string * limit: string
    | LengthGreaterThanLimit of field: string * limit: string
    | InvalidValue of field: string * value: string
    | InvalidEmailAddress of field: string
    | InvalidPhoneNumber of field: string
    | PatternNotMatched of field: string

    static member toString (error: ValidationError) =
        match error with
        | ValueNull field -> $"'%s{field}' must not be null."
        | ValueNullOrEmpty field -> $"'%s{field}' must not be null or empty."
        | ValueLessThanLimit (field, limit) -> $"'%s{field}' must be greater than or equal to %s{limit}."
        | ValueGreaterThanLimit (field, limit) -> $"'%s{field}' must be less than or equal to %s{limit}."
        | LengthLessThanLimit (field, limit) -> $"The length of '%s{field}' must be at least %s{limit} character(s)."
        | LengthGreaterThanLimit (field, limit) -> $"The length of '%s{field}' must be %s{limit} character(s) or fewer."
        | InvalidValue (field, value) -> $"'%s{field}' cannot contain '%s{value}'."
        | InvalidEmailAddress field -> $"'%s{field}' is not a valid email address."
        | InvalidPhoneNumber field -> $"'%s{field}' is not a valid phone number."
        | PatternNotMatched field -> $"'%s{field}' is not in the correct format."

type PersistenceError =
    | NotFound of entity: string
    | DtoConversion of dto: string * error: ValidationError
    | Database of operation: string * ex: exn // Introduce separate DU cases for database errors that are important for domain and control flow (see NotFound).
    | Other of operation: string * ex: exn

    static member notFound entity = NotFound(entity)
    static member notFoundResult entity = Error(NotFound(entity))
    static member dtoConversion dto error = DtoConversion(dto, error)
    static member database operation ex = Database(operation, ex)
    static member other operation ex = Other(operation, ex)

    static member toString (error: PersistenceError) =
        match error with
        | NotFound entity -> $"%s{entity} is not found."
        | DtoConversion (dto, error) -> $"Failed to convert '%s{dto}': %s{ValidationError.toString error}"
        | Database (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
        | Other (operation, ex) -> $"Failed to %s{operation}: %s{ex.Message}"
