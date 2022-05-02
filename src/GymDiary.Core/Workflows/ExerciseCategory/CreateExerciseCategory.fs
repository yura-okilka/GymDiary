namespace GymDiary.Core.Workflows.ExerciseCategory

open System

open GymDiary.Core.Domain
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module CreateExerciseCategory =

    type Command = { Name: string; OwnerId: string }

    type CommandResult =
        { Id: string }

        static member create id = { Id = id }

    type CommandError =
        | Validation of ValidationError list
        | Domain of DomainError
        | Persistence of PersistenceError

        static member validation e = Validation e
        static member domain e = Domain e
        static member domainResult e = Error(Domain e)
        static member persistence e = Persistence e

        static member toString (error: CommandError) =
            match error with
            | Validation e -> e |> List.map ValidationError.toString |> String.concat Environment.NewLine
            | Domain e -> e |> DomainError.toString
            | Persistence e -> e |> PersistenceError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingContext =
        { ErrorEventId = Events.ExerciseCategoryCreationFailed
          GetErrorMessage = fun err -> err |> CommandError.toString
          GetRequestInfo =
            fun cmd ->
                Map [ (nameof cmd.Name, cmd.Name)
                      (nameof cmd.OwnerId, cmd.OwnerId) ] }

    let createWorkflow
        (categoryWithNameExistsInDB: String50 -> Async<Result<bool, PersistenceError>>)
        (sportsmanWithIdExistsInDB: SportsmanId -> Async<Result<bool, PersistenceError>>)
        (createCategoryInDB: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>)
        (logger: ILogger)
        : Workflow =
        fun command ->
            asyncResult {
                let! category =
                    validation {
                        let! id = ExerciseCategoryId.Empty |> Ok
                        and! name = String50.create (nameof command.Name) command.Name
                        and! ownerId = SportsmanId.create (nameof command.OwnerId) command.OwnerId
                        return ExerciseCategory.create id name ownerId
                    }
                    |> Result.mapError CommandError.validation

                let! ownerExists =
                    sportsmanWithIdExistsInDB category.OwnerId |> AsyncResult.mapError CommandError.persistence

                if not ownerExists then
                    return! OwnerNotFound |> CommandError.domainResult

                let! categoryExists =
                    categoryWithNameExistsInDB category.Name |> AsyncResult.mapError CommandError.persistence

                if categoryExists then
                    return!
                        ExerciseCategoryWithNameAlreadyExists(category.Name |> String50.value)
                        |> CommandError.domainResult

                let! id = createCategoryInDB category |> AsyncResult.mapError CommandError.persistence
                let rawId = id |> ExerciseCategoryId.value

                logger.LogInformation(
                    Events.ExerciseCategoryCreated,
                    "Exercise category was created with id '{id}'.",
                    rawId
                )

                return rawId |> CommandResult.create
            }
