namespace GymDiary.Core.Workflows

open Microsoft.Extensions.Logging

module ErrorLoggingDecorator =

    type ErrorInfo =
        { Message: string
          Exception: exn option }

    type LoggingContext<'Request, 'Error> =
        { GetRequestInfo: 'Request -> Map<string, obj>
          GetErrorInfo: 'Error -> ErrorInfo
          ErrorEventId: EventId }

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
                        let messageTemplate = "Workflow failed with error: {error}"
                        let errorInfo = context.GetErrorInfo error

                        match errorInfo.Exception with
                        | Some ex -> logger.LogError(context.ErrorEventId, ex, messageTemplate, errorInfo.Message)
                        | None -> logger.LogError(context.ErrorEventId, messageTemplate, errorInfo.Message)

                    return result
                with
                | ex ->
                    logger.LogError(context.ErrorEventId, ex, "Workflow failed with exception: {exception}", ex.Message)
                    return raise (exn ("Failed to run workflow.", ex))
            }
