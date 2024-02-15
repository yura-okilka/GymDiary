namespace GymDiary.Api

open GymDiary.Core.Domain

type ErrorResponse = {
    Name: string
    Message: string
    Details: obj
}

module Responses =

    let internalError = {
        Name = "ServerError"
        Message = "Server error has occurred"
        Details = null
    }

    let parsingError = {
        Name = "ParsingError"
        Message = "Request cannot be parsed"
        Details = null
    }

    let validationError (error: ValidationError) = {
        Name = "ValidationError"
        Message = ValidationError.toString error
        Details = null
    }

    let validationErrors (errors: ValidationError list) = {
        Name = "ValidationErrors"
        Message = "Validation errors have occurred"
        Details =
            errors
            |> List.map (fun (ValidationError(field, _) as error) -> (field, ValidationError.toString error))
            |> Map.ofList
    }

    let exerciseCategoryNotFound (error: ExerciseCategoryNotFoundError) = {
        Name = "ExerciseCategoryNotFound"
        Message = ExerciseCategoryNotFoundError.toString error
        Details = null
    }

    let exerciseCategoryAlreadyExists (error: ExerciseCategoryAlreadyExistsError) = {
        Name = "ExerciseCategoryAlreadyExists"
        Message = ExerciseCategoryAlreadyExistsError.toString error
        Details = null
    }

    let nameAlreadyUsed (error: ExerciseCategoryAlreadyExistsError) = {
        Name = "NameAlreadyUsed"
        Message = ExerciseCategoryAlreadyExistsError.toString error
        Details = null
    }

    let ownerNotFound (error: OwnerNotFoundError) = {
        Name = "OwnerNotFound"
        Message = OwnerNotFoundError.toString error
        Details = null
    }
