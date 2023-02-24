namespace GymDiary.Core.Workflows

open Microsoft.Extensions.Logging

module ErrorLoggingDecorator =

    type ILoggingInfoProvider<'Request, 'Error> =
        abstract ErrorEventId: EventId
        abstract GetRequestInfo: 'Request -> Map<string, obj>
        abstract GetErrorMessage: 'Error -> string

    let logWorkflow
        (logger: ILogger)
        (loggingInfo: ILoggingInfoProvider<'Request, 'Error>)
        (workflow: Workflow<'Request, _, 'Error>)
        : Workflow<'Request, _, 'Error> =
        fun request -> async {
            try
                use _ = logger.BeginScope(loggingInfo.GetRequestInfo request)

                let! result = workflow request

                match result with
                | Ok _ -> ()
                | Error error ->
                    let message = loggingInfo.GetErrorMessage error
                    logger.LogError(loggingInfo.ErrorEventId, "Workflow failed with error: {error}", message) // TODO: use structured logging.

                return result
            with ex ->
                logger.LogError(loggingInfo.ErrorEventId, ex, "Workflow failed with exception: {exception}", ex.Message)

                return raise (exn ("Failed to execute workflow", ex))
        }
