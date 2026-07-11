namespace GymDiary.Application.ExerciseCategories

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Domain.Users
open GymDiary.Application.Time
open GymDiary.Application.Persistence
open GymDiary.Application.Validation
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes

module CreateExerciseCategoryWorkflow =

    type public Command = { Name: string; OwnerId: Guid }

    type public CommandError =
        | InvalidCommand of ValidationError list
        | CategoryAlreadyExists of name: String50
        | OwnerNotFound of id: UserId

    type public Handler
        (
            timeProvider: TimeProvider,
            userRepository: IUserRepository,
            categoryRepository: IExerciseCategoryRepository
        ) =
        member _.Handle command =
            asyncResult {
                let categoryId = EntityId.create<exerciseCategoryId> ()
                let ownerId: UserId = %command.OwnerId

                let! name =
                    command.Name
                    |> Validation.checkField (nameof command.Name) String50.create
                    |> Result.mapError (List.singleton >> InvalidCommand)

                let! ownerExists = userRepository.ExistsWithId ownerId

                if not ownerExists then
                    return! Error(OwnerNotFound ownerId)

                let! categoryExists = categoryRepository.ExistsWithName name ownerId

                if categoryExists then
                    return! Error(CategoryAlreadyExists name)

                let category = ExerciseCategory.create categoryId name ownerId timeProvider.UtcNow
                do! categoryRepository.Create category

                return %category.Id
            }
            |> Async.StartAsTask

module CreateExerciseCategoryResultExtensions =

    [<Extension>]
    let Match
        (
            result: Result<Guid, CreateExerciseCategoryWorkflow.CommandError>,
            onOk: Func<_, _>,
            onInvalidCommand: Func<_, _>,
            onCategoryAlreadyExists: Func<_, _>,
            onOwnerNotFound: Func<_, _>
        ) =
        match result with
        | Ok v -> onOk.Invoke(v)
        | Error error ->
            match error with
            | CreateExerciseCategoryWorkflow.InvalidCommand e -> onInvalidCommand.Invoke(e)
            | CreateExerciseCategoryWorkflow.CategoryAlreadyExists e -> onCategoryAlreadyExists.Invoke(e)
            | CreateExerciseCategoryWorkflow.OwnerNotFound e -> onOwnerNotFound.Invoke(e)
