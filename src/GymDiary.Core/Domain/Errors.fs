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

    static member toString (field: string) (error: ValueError) =
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

type ValidationError =
    | ValidationError of field: string * error: ValueError

    static member toString (ValidationError (field, error)) = ValueError.toString field error

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

    static member getException (error: PersistenceError) =
        match error with
        | Database (_, ex) -> ex |> Some
        | Other (_, ex) -> ex |> Some
        | _ -> None
