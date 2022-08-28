namespace GymDiary.Core.Workflows

open Microsoft.Extensions.Logging

module ErrorLoggingDecorator =

    type LoggingContext<'Request, 'Error> =
        {
            GetRequestInfo: 'Request -> Map<string, obj>
            GetErrorMessage: 'Error -> string
            ErrorEventId: EventId
        }

    let logWorkflow
        (logger: ILogger)
        (context: LoggingContext<'Request, 'Error>)
        (workflow: Workflow<'Request, _, 'Error>)
        : Workflow<'Request, _, 'Error> =
        fun request ->
            async {
                try
                    use _ = logger.BeginScope(context.GetRequestInfo request)

                    let! result = workflow request

                    match result with
                    | Ok _ -> ()
                    | Error error ->
                        let message = context.GetErrorMessage error
                        logger.LogError(context.ErrorEventId, "Workflow failed with error: {error}", message) // TODO: use structured logging.

                    return result
                with
                | ex ->
                    logger.LogError(context.ErrorEventId, ex, "Workflow failed with exception: {exception}", ex.Message)
                    return raise (exn ("Failed to run workflow", ex))
            }
