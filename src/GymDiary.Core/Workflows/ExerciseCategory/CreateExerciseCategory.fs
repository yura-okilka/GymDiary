namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module CreateExerciseCategory =

    type Command = { Name: string; OwnerId: string }

    type CommandResult = { Id: string }

    type CommandError =
        | InvalidCommand of ValidationError list
        | CategoryAlreadyExists of ExerciseCategoryAlreadyExistsError
        | OwnerNotFound of OwnerNotFoundError

        static member categoryAlreadyExists name =
            ExerciseCategoryAlreadyExistsError.create name |> CategoryAlreadyExists |> Error

        static member ownerNotFound id = OwnerNotFoundError.create id |> OwnerNotFound |> Error

        static member toString error =
            match error with
            | InvalidCommand es -> es |> ValidationErrors.toString
            | CategoryAlreadyExists e -> e |> ExerciseCategoryAlreadyExistsError.toString
            | OwnerNotFound e -> e |> OwnerNotFoundError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.ExerciseCategoryCreationFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) =
                Map [
                    (nameof command.Name, command.Name)
                    (nameof command.OwnerId, command.OwnerId)
                ]
        }

    let execute
        (idProvider: IIdProvider)
        (categoryWithNameExistsInDB: String50 -> UserId -> Async<bool>)
        (userWithIdExistsInDB: UserId -> Async<bool>)
        (createCategoryInDB: ExerciseCategory -> Async<ExerciseCategoryId>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! category =
                validation {
                    let! id = idProvider.GenerateId() |> Ok
                    and! name = String50.create (nameof command.Name) command.Name
                    and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                    return ExerciseCategoryAggregate.create id name ownerId
                }
                |> Result.mapError InvalidCommand

            let! ownerExists = userWithIdExistsInDB category.OwnerId

            if not ownerExists then
                return! CommandError.ownerNotFound category.OwnerId

            let! categoryExists = categoryWithNameExistsInDB category.Name category.OwnerId

            if categoryExists then
                return! CommandError.categoryAlreadyExists category.Name

            let! categoryId = createCategoryInDB category |> Async.map Id.value

            logger.LogInformation(DomainEvents.ExerciseCategoryCreated, "Exercise category was created with id '{id}'", categoryId)

            return { Id = categoryId }
        }
