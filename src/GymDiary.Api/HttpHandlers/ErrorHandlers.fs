namespace GymDiary.Api.HttpHandlers

open System
open System.Text.Json

open Giraffe

open GymDiary.Api
open Microsoft.Extensions.Logging

module ErrorHandlers =

    let unknownError (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request")
        clearResponse >=> ServerErrors.INTERNAL_ERROR Responses.internalError

    let parsingError (ex: JsonException) (logger: ILogger) =
        logger.LogError(ex, "Failed to parse JSON request")
        clearResponse >=> RequestErrors.BAD_REQUEST Responses.parsingError
