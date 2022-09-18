namespace GymDiary.Core.Workflows.Exercise

open System

open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.CommonDtos
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module CreateExercise =

    type Command =
        {
            CategoryId: string
            OwnerId: string
            Name: string
            Notes: string option
            RestTime: TimeSpan
            Sets: ExerciseSetsDto
        }

    type CommandResult = { Id: string }

    type CommandError =
        | InvalidCommand of ValidationError list
        | CategoryNotFound of ExerciseCategoryNotFoundError
        | OwnerNotFound of OwnerNotFoundError

        static member categoryNotFound id ownerId =
            ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

        static member ownerNotFound id = OwnerNotFoundError.create id |> OwnerNotFound |> Error

        static member toString error =
            match error with
            | InvalidCommand es -> es |> List.map ValidationError.toString |> String.concat " " // TODO: ValidationErrors.toString
            | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString
            | OwnerNotFound e -> e |> OwnerNotFoundError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.ExerciseCreationFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) =
                Map [ (nameof command.Name, command.Name)
                      (nameof command.OwnerId, command.OwnerId) ]
        }

    let execute
        (getCategoryByIdFromDB: SportsmanId -> ExerciseCategoryId -> Async<ExerciseCategory option>)
        (sportsmanWithIdExistsInDB: SportsmanId -> Async<bool>)
        (createExerciseInDB: Exercise -> Async<ExerciseId>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! validated =
                validation {
                    let! categoryId = Id.create (nameof command.CategoryId) command.CategoryId
                    and! ownerId = Id.create (nameof command.OwnerId) command.OwnerId
                    and! name = String50.create (nameof command.Name) command.Name
                    and! notes = Option.traverseResult (String1k.create (nameof command.Notes)) command.Notes
                    and! sets = ExerciseSetsDto.toDomain command.Sets

                    return
                        {|
                            CategoryId = categoryId
                            OwnerId = ownerId
                            Name = name
                            Notes = notes
                            Sets = sets
                        |}
                }
                |> Result.mapError InvalidCommand

            let! _ =
                getCategoryByIdFromDB validated.OwnerId validated.CategoryId
                |> AsyncResult.requireSome (CommandError.categoryNotFound validated.CategoryId validated.OwnerId)

            let! ownerExists = sportsmanWithIdExistsInDB validated.OwnerId

            if not ownerExists then
                return! CommandError.ownerNotFound validated.OwnerId

            let exercise =
                Exercise.create
                    validated.CategoryId
                    validated.OwnerId
                    validated.Name
                    validated.Notes
                    command.RestTime
                    validated.Sets
                    DateTime.UtcNow // TODO: ITimeProvider

            let! exerciseId = createExerciseInDB exercise |> Async.map Id.value

            logger.LogInformation(DomainEvents.ExerciseCreated, "Exercise was created with id '{id}'", exerciseId)

            return { Id = exerciseId }
        }
