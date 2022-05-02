namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module RenameExerciseCategory =

    type Command = { Id: string; Name: string }

    type CommandError =
        | Validation of ValidationError
        | Domain of DomainError
        | Persistence of PersistenceError

        static member validation e = Validation e
        static member domain e = Domain e
        static member domainResult e = Error(Domain e)
        static member persistence e = Persistence e

        static member toString (error: CommandError) =
            match error with
            | Validation e -> e |> ValidationError.toString
            | Domain e -> e |> DomainError.toString
            | Persistence e -> e |> PersistenceError.toString

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingContext =
        { ErrorEventId = Events.ExerciseCategoryRenamingFailed
          GetErrorMessage = fun err -> err |> CommandError.toString
          GetRequestInfo =
            fun cmd ->
                Map [ (nameof cmd.Id, cmd.Id)
                      (nameof cmd.Name, cmd.Name) ] }

    let createWorkflow
        (getCategoryByIdFromDB: ExerciseCategoryId -> Async<Result<ExerciseCategory, PersistenceError>>)
        (categoryWithNameExistsInDB: String50 -> Async<Result<bool, PersistenceError>>)
        (updateCategoryInDB: ExerciseCategory -> Async<Result<unit, PersistenceError>>)
        (logger: ILogger)
        : Workflow =
        fun command ->
            asyncResult {
                let! id =
                    ExerciseCategoryId.create (nameof command.Id) command.Id
                    |> Result.setError (ExerciseCategoryNotFound |> CommandError.domain)

                let! name =
                    String50.create (nameof command.Name) command.Name |> Result.mapError CommandError.validation

                let! categoryExists = categoryWithNameExistsInDB name |> AsyncResult.mapError CommandError.persistence

                if categoryExists then
                    return! ExerciseCategoryWithNameAlreadyExists(name |> String50.value) |> CommandError.domainResult

                let! category =
                    getCategoryByIdFromDB id
                    |> AsyncResult.mapError (fun error ->
                        match error with
                        | NotFound _ -> ExerciseCategoryNotFound |> CommandError.domain
                        | _ -> error |> CommandError.persistence)

                let renamedCategory = category |> ExerciseCategory.rename name

                do! updateCategoryInDB renamedCategory |> AsyncResult.mapError CommandError.persistence

                logger.LogInformation(
                    Events.ExerciseCategoryRenamed,
                    "Exercise category with id '{id}' was renamed to '{name}'.",
                    id |> ExerciseCategoryId.value,
                    name |> String50.value
                )
            }
