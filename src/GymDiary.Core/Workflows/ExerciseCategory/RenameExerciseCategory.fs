namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator
open GymDiary.Core.Persistence

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module RenameExerciseCategory =

    type Command =
        {
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
            | InvalidCommand es -> es |> List.map ValidationError.toString |> String.concat " "
            | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString
            | NameAlreadyUsed e -> e |> ExerciseCategoryAlreadyExistsError.toString

    type Workflow = Workflow<Command, unit, CommandError>

    let LoggingContext =
        {
            ErrorEventId = Events.ExerciseCategoryRenamingFailed
            GetErrorMessage = CommandError.toString
            GetRequestInfo =
                fun cmd ->
                    Map [ (nameof cmd.Id, cmd.Id)
                          (nameof cmd.OwnerId, cmd.OwnerId)
                          (nameof cmd.Name, cmd.Name) ]
        }

    let runWorkflow
        (getCategoryByIdFromDB: SportsmanId -> ExerciseCategoryId -> Async<ExerciseCategory option>)
        (categoryWithNameExistsInDB: SportsmanId -> String50 -> Async<bool>)
        (updateCategoryInDB: ExerciseCategory -> ModifyEntityResult)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! (categoryId, ownerId, name) =
                validation {
                    let! categoryId = Id.create (nameof command.Id) command.Id
                    and! ownerId = Id.create (nameof command.OwnerId) command.OwnerId
                    and! name = String50.create (nameof command.Name) command.Name
                    return (categoryId, ownerId, name)
                }
                |> Result.mapError InvalidCommand

            let! categoryExists = categoryWithNameExistsInDB ownerId name

            if categoryExists then
                return! CommandError.nameAlreadyUsed name

            let! category =
                getCategoryByIdFromDB ownerId categoryId
                |> AsyncResult.requireSome (CommandError.categoryNotFound categoryId ownerId)

            let renamedCategory = category |> ExerciseCategory.rename name

            do!
                updateCategoryInDB renamedCategory
                |> AsyncResult.mapError (function
                    | EntityNotFound _ -> CommandError.categoryNotFound renamedCategory.Id renamedCategory.OwnerId)

            logger.LogInformation(
                Events.ExerciseCategoryRenamed,
                "Exercise category with id '{id}' was renamed to '{name}'",
                command.Id,
                command.Name
            )
        }
