namespace GymDiary.Api

open System

open GymDiary.Core.Domain
open GymDiary.Core.Domain.Errors

open Giraffe

open Microsoft.Extensions.Logging

type ErrorResponse =
    { Name: string
      Message: string
      Details: obj }

    static member from (error: DomainError) =
        { Name = "DomainError"
          Message = error |> DomainError.toString
          Details = null }

    static member from (error: ValidationError) =
        { Name = "ValidationError"
          Message = error |> ValidationError.toString
          Details = null }

    static member from (errors: ValidationError list) =
        { Name = "ValidationErrors"
          Message = "Validation errors have occurred."
          Details = errors |> List.map ValidationError.toString }

    static member from (error: PersistenceError) =
        { Name = "PersistenceError"
          Message = error |> PersistenceError.toString
          Details = null }

    static member internalError =
        { Name = "ServerError"
          Message = "Server error has occurred."
          Details = null }

module ErrorHandler =

    let handle (ex: Exception) (logger: ILogger) =
        logger.LogError(Events.undefinedFailure, ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> ServerErrors.INTERNAL_ERROR ErrorResponse.internalError
