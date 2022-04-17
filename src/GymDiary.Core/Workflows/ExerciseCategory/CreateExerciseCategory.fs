namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes

open FsToolkit.ErrorHandling

module CreateExerciseCategory =

    type Command =
        { Name: string
          OwnerId: string }

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

    type Workflow = Command -> Async<Result<CommandResult, CommandError>>

    let createWorkflow
        (categoryWithNameExistsInDB: String50 -> Async<Result<bool, PersistenceError>>)
        (sportsmanWithIdExistsInDB: SportsmanId -> Async<Result<bool, PersistenceError>>)
        (createCategoryInDB: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>)
        : Workflow =
        fun command ->
            asyncResult {
                let! category =
                    validation {
                        let! id = ExerciseCategoryId.empty |> Ok
                        and! name = String50.create (nameof command.Name) command.Name
                        and! ownerId = SportsmanId.create (nameof command.OwnerId) command.OwnerId
                        return ExerciseCategory.create id name ownerId
                    }
                    |> Result.mapError CommandError.validation

                let! ownerExists =
                    sportsmanWithIdExistsInDB category.OwnerId |> AsyncResult.mapError CommandError.persistence

                if not ownerExists then
                    return! OwnerIsNotFound |> CommandError.domainResult

                let! categoryExists =
                    categoryWithNameExistsInDB category.Name |> AsyncResult.mapError CommandError.persistence

                if categoryExists then
                    return! ExerciseCategoryAlreadyExists |> CommandError.domainResult

                let! id = createCategoryInDB category |> AsyncResult.mapError CommandError.persistence

                return id |> ExerciseCategoryId.value |> CommandResult.create
            }
