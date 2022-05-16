namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module DeleteExerciseCategory =

    type Command = { Id: string }

    type CommandError =
        | Domain of DomainError
        | Persistence of PersistenceError

        static member domain e = Domain e
        static member persistence e = Persistence e

        static member toString (error: CommandError) =
            match error with
            | Domain e -> e |> DomainError.toString
            | Persistence e -> e |> PersistenceError.toString

        static member getException (error: CommandError) =
            match error with
            | Persistence e -> e |> PersistenceError.getException
            | _ -> None

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingContext =
        { ErrorEventId = Events.ExerciseCategoryDeletionFailed
          GetErrorInfo =
            fun err ->
                { Message = err |> CommandError.toString
                  Exception = err |> CommandError.getException }
          GetRequestInfo = fun cmd -> Map [ (nameof cmd.Id, cmd.Id) ] }

    let createWorkflow
        (deleteCategoryFromDB: ExerciseCategoryId -> Async<Result<unit, PersistenceError>>)
        (logger: ILogger)
        : Workflow =
        fun command ->
            asyncResult {
                let! id =
                    ExerciseCategoryId.create (nameof command.Id) command.Id
                    |> Result.setError (ExerciseCategoryNotFound |> CommandError.domain)

                do!
                    deleteCategoryFromDB id
                    |> AsyncResult.mapError (fun error ->
                        match error with
                        | EntityNotFound _ -> ExerciseCategoryNotFound |> CommandError.domain
                        | _ -> error |> CommandError.persistence)

                logger.LogInformation(
                    Events.ExerciseCategoryDeleted,
                    "Exercise category with id '{id}' was deleted.",
                    id |> ExerciseCategoryId.value
                )
            }
