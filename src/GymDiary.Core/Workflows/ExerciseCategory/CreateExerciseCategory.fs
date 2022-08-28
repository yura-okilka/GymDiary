namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
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
            | InvalidCommand es -> es |> List.map ValidationError.toString |> String.concat " "
            | CategoryAlreadyExists e -> e |> ExerciseCategoryAlreadyExistsError.toString
            | OwnerNotFound e -> e |> OwnerNotFoundError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingContext =
        {
            ErrorEventId = Events.ExerciseCategoryCreationFailed
            GetErrorMessage = CommandError.toString
            GetRequestInfo =
                fun cmd ->
                    Map [ (nameof cmd.Name, cmd.Name)
                          (nameof cmd.OwnerId, cmd.OwnerId) ]
        }

    let runWorkflow
        (categoryWithNameExistsInDB: SportsmanId -> String50 -> Async<bool>)
        (sportsmanWithIdExistsInDB: SportsmanId -> Async<bool>)
        (createCategoryInDB: ExerciseCategory -> Async<ExerciseCategoryId>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! category =
                validation {
                    let! id = Id.Empty |> Ok
                    and! name = String50.create (nameof command.Name) command.Name
                    and! ownerId = Id.create (nameof command.OwnerId) command.OwnerId
                    return ExerciseCategory.create id name ownerId
                }
                |> Result.mapError InvalidCommand

            let! ownerExists = sportsmanWithIdExistsInDB category.OwnerId

            if not ownerExists then
                return! CommandError.ownerNotFound category.OwnerId

            let! categoryExists = categoryWithNameExistsInDB category.OwnerId category.Name

            if categoryExists then
                return! CommandError.categoryAlreadyExists category.Name

            let! categoryId = createCategoryInDB category |> Async.map Id.value

            logger.LogInformation(
                Events.ExerciseCategoryCreated,
                "Exercise category was created with id '{id}'",
                categoryId
            )

            return { Id = categoryId }
        }
