namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core
open GymDiary.Core.Domain
open GymDiary.Core.Domain.Logic
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module RenameExerciseCategory =

    type Command =
        { Id: string
          OwnerId: string
          Name: string }

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

        static member getException (error: CommandError) =
            match error with
            | Persistence e -> e |> PersistenceError.getException
            | _ -> None

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingContext =
        { ErrorEventId = Events.ExerciseCategoryRenamingFailed
          GetErrorInfo =
            fun err ->
                { Message = err |> CommandError.toString
                  Exception = err |> CommandError.getException }
          GetRequestInfo =
            fun cmd ->
                Map [ (nameof cmd.Id, cmd.Id)
                      (nameof cmd.Name, cmd.Name) ] }

    let runWorkflow
        (getCategoryByIdFromDB: SportsmanId -> ExerciseCategoryId -> PersistenceResult<ExerciseCategory>)
        (categoryWithNameExistsInDB: SportsmanId -> String50 -> PersistenceResult<bool>)
        (updateCategoryInDB: ExerciseCategory -> PersistenceResult<unit>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! (categoryId, ownerId) =
                result {
                    let! categoryId = ExerciseCategoryId.create (nameof command.Id) command.Id
                    let! ownerId = SportsmanId.create (nameof command.OwnerId) command.OwnerId
                    return (categoryId, ownerId)
                }
                |> Result.setError (ExerciseCategoryNotFound |> CommandError.domain)

            let! name = String50.create (nameof command.Name) command.Name |> Result.mapError CommandError.validation

            let! categoryExists =
                categoryWithNameExistsInDB ownerId name |> AsyncResult.mapError CommandError.persistence

            if categoryExists then
                return! ExerciseCategoryWithNameAlreadyExists(name |> String50.value) |> CommandError.domainResult

            let! category =
                getCategoryByIdFromDB ownerId categoryId
                |> AsyncResult.mapError (fun error ->
                    match error with
                    | EntityNotFound _ -> ExerciseCategoryNotFound |> CommandError.domain
                    | _ -> error |> CommandError.persistence)

            let renamedCategory = category |> ExerciseCategory.rename name

            do! updateCategoryInDB renamedCategory |> AsyncResult.mapError CommandError.persistence

            logger.LogInformation(
                Events.ExerciseCategoryRenamed,
                "Exercise category with id '{id}' was renamed to '{name}'.",
                categoryId |> ExerciseCategoryId.value,
                name |> String50.value
            )
        }
