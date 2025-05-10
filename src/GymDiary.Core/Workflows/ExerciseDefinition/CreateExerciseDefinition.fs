module GymDiary.Core.Workflows.ExerciseDefinition.CreateExerciseDefinition

open System

open FsToolkit.ErrorHandling
open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Persistence
open Microsoft.Extensions.Logging

type Command = {
    CategoryId: string
    OwnerId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
// TODO: add Sets: ExerciseSetsDto
}

type CommandError =
    | InvalidCommand of ValidationError list
    | CategoryNotFound of ExerciseCategoryNotFoundError
    | OwnerNotFound of OwnerNotFoundError

    static member categoryNotFound id ownerId =
        ExerciseCategoryNotFoundError.create id ownerId |> CategoryNotFound

    static member ownerNotFound id = OwnerNotFoundError.create id |> OwnerNotFound

    static member toString error =
        match error with
        | InvalidCommand es -> es |> ValidationErrors.toString
        | CategoryNotFound e -> e |> ExerciseCategoryNotFoundError.toString
        | OwnerNotFound e -> e |> OwnerNotFoundError.toString

type ICommandHandler = IRequestHandler<Command, string, CommandError>

type CommandHandler
    (
        idProvider: IIdProvider,
        userRepository: IUserRepository,
        categoryRepository: IExerciseCategoryRepository,
        exerciseRepository: IExerciseDefinitionRepository,
        logger: ILogger
    ) =
    interface ICommandHandler with

        member _.Handle command =
            asyncResult {
                let! validated =
                    validation {
                        let! categoryId = Id.tryCreate (nameof command.CategoryId) command.CategoryId
                        and! ownerId = Id.tryCreate (nameof command.OwnerId) command.OwnerId
                        and! name = String50.create (nameof command.Name) command.Name
                        and! notes = Option.traverseResult (String1k.create (nameof command.Notes)) command.Notes

                        let sets = {
                            Type = ExerciseSetType.Duration
                            Items = []
                        }

                        return {|
                            CategoryId = categoryId
                            OwnerId = ownerId
                            Name = name
                            Notes = notes
                            Sets = sets
                        |}
                    }
                    |> Result.mapError InvalidCommand

                let! _ =
                    categoryRepository.Get validated.CategoryId validated.OwnerId
                    |> Async.AwaitTask
                    |> AsyncResult.requireSome (CommandError.categoryNotFound validated.CategoryId validated.OwnerId)

                let! ownerExists = userRepository.ExistWithId validated.OwnerId

                if not ownerExists then
                    return! CommandError.ownerNotFound validated.OwnerId |> Error

                let exercise =
                    ExerciseDefinitionAggregate.create
                        (idProvider.GenerateId())
                        validated.CategoryId
                        validated.OwnerId
                        validated.Name
                        validated.Notes
                        command.RestTime
                        validated.Sets
                        DateTime.UtcNow // TODO: ITimeProvider

                do! exerciseRepository.Create exercise

                logger.LogInformation("Exercise definition was created with id '{id}'", exercise.Id)

                return exercise.Id |> Id.value
            }
            |> Async.StartAsTask
