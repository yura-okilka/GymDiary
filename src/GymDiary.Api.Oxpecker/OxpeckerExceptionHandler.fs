namespace GymDiary.Api

open System
open System.Threading
open System.Threading.Tasks
open Oxpecker
open Microsoft.AspNetCore.Diagnostics
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

type OxpeckerExceptionHandler(problemDetailsService: IProblemDetailsService) =
    interface IExceptionHandler with
        member _.TryHandleAsync(httpContext: HttpContext, ex: Exception, cancellationToken: CancellationToken) : ValueTask<bool> =
            task {
                // TODO: use in .NET 9 https://www.milanjovanovic.tech/blog/problem-details-for-aspnetcore-apis#handling-specific-exceptions-status-codes
                match ex with
                | :? ModelBindException
                | :? RouteParseException as ex ->
                    let problemContext =
                        ProblemDetailsContext(
                            Exception = ex,
                            HttpContext = httpContext,
                            ProblemDetails = ProblemDetails(Status = StatusCodes.Status400BadRequest, Detail = ex.Message)
                        )

                    return! problemDetailsService.TryWriteAsync(problemContext)
                | _ -> return false
            }
            |> ValueTask<bool>
