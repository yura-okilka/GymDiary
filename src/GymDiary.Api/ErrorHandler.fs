namespace GymDiary.Api

open System

open GymDiary.Core.Domain

open Giraffe

open Microsoft.Extensions.Logging

type ErrorResponse =
    { Name: string
      Message: string
      Details: obj }

    static member validationError (error: ValidationError) =
        { Name = "ValidationError"
          Message = ValidationError.toString error
          Details = null }

    static member validationErrors (errors: ValidationError list) =
        { Name = "ValidationErrors"
          Message = "Validation errors have occurred."
          Details =
            errors
            |> List.map (fun (ValidationError (field, _) as error) -> (field, ValidationError.toString error))
            |> Map.ofList }

    static member exerciseCategoryAlreadyExists message =
        { Name = "ExerciseCategoryAlreadyExists"
          Message = message
          Details = null }

    static member exerciseCategoryNotFound message =
        { Name = "ExerciseCategoryNotFound"
          Message = message
          Details = null }

    static member ownerNotFound message =
        { Name = "OwnerNotFound"
          Message = message
          Details = null }

    static member nameAlreadyUsed message =
        { Name = "NameAlreadyUsed"
          Message = message
          Details = null }

    static member InternalError =
        { Name = "ServerError"
          Message = "Server error has occurred."
          Details = null }

module ErrorHandler =

    let handle (ex: Exception) (logger: ILogger) =
        logger.LogError(Events.UndefinedFailure, ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> ServerErrors.INTERNAL_ERROR ErrorResponse.InternalError
