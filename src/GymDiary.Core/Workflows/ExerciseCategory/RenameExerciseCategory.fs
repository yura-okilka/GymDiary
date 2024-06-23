namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator
open GymDiary.Core.Persistence

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module RenameExerciseCategory =

    type Command = {
        Id: string
        OwnerId: string
        Name: string
    }

    type CommandError =
        | InvalidCommand of ValidationError list
        | CategoryNotFound of ExerciseCategoryNotFoundError
        | NameAlreadyUsed of ExerciseCategoryAlreadyExistsError

        static member categoryNotFound id ownerId =
            ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

        static member nameAlreadyUsed name =
            ExerciseCategoryAlreadyExistsError.create name |> NameAlreadyUsed |> Error

        static member toString error =
            match error with
            | InvalidCommand es -> es |> ValidationErrors.toString
            | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString
            | NameAlreadyUsed e -> e |> ExerciseCategoryAlreadyExistsError.toString

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.ExerciseCategoryRenamingFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) =
                Map [
                    (nameof command.Id, command.Id)
                    (nameof command.OwnerId, command.OwnerId)
                    (nameof command.Name, command.Name)
                ]
        }

    let execute
        (getCategoryByIdFromDB: ExerciseCategoryId -> UserId -> Async<ExerciseCategory option>)
        (categoryWithNameExistsInDB: String50 -> UserId -> Async<bool>)
        (updateCategoryInDB: ExerciseCategory -> Async<UpdateEntityResult>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! (categoryId, ownerId, name) =
                validation {
                    let! categoryId = Id.tryCreate (nameof command.Id) command.Id
                    and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                    and! name = String50.create (nameof command.Name) command.Name
                    return (categoryId, ownerId, name)
                }
                |> Result.mapError InvalidCommand

            let! categoryExists = categoryWithNameExistsInDB name ownerId

            if categoryExists then
                return! CommandError.nameAlreadyUsed name

            let! category =
                getCategoryByIdFromDB categoryId ownerId
                |> AsyncResult.requireSome (CommandError.categoryNotFound categoryId ownerId)

            let renamedCategory = category |> ExerciseCategory.rename name

            do!
                updateCategoryInDB renamedCategory
                |> AsyncResult.mapError (function
                    | EntityNotFound _ -> CommandError.categoryNotFound renamedCategory.Id renamedCategory.OwnerId)

            logger.LogInformation(
                DomainEvents.ExerciseCategoryRenamed,
                "Exercise category with id '{id}' was renamed to '{name}'",
                command.Id,
                command.Name
            )
        }
