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
        | Validation of ValidationError
        | Domain of DomainError
        | Persistence of PersistenceError

        static member validation e = Validation e
        static member domain e = Domain e
        static member domainResult e = Error(Domain e)
        static member persistence e = Persistence e

    type Workflow = Command -> Async<Result<CommandResult, CommandError>>

    let createWorkflow
        (categoryWithNameExistsInDB: string -> Async<Result<bool, PersistenceError>>)
        (createCategoryInDB: ExerciseCategory -> Async<Result<ExerciseCategoryId, PersistenceError>>)
        : Workflow =
        fun command ->
            asyncResult {
                let! category =
                    result { // TODO: use validation CE to collect errors.
                        let! id = ExerciseCategoryId.empty
                        let! name = String50.create (nameof command.Name) command.Name
                        let! ownerId = SportsmanId.create (nameof command.OwnerId) command.OwnerId // TODO: ensure sportsman exists in DB.
                        return ExerciseCategory.create id name ownerId
                    }
                    |> Result.mapError CommandError.validation

                let! alreadyExists =
                    categoryWithNameExistsInDB command.Name |> AsyncResult.mapError CommandError.persistence

                if alreadyExists then
                    return! ExerciseCategoryAlreadyExists |> CommandError.domainResult
                else
                    let! id = createCategoryInDB category |> AsyncResult.mapError CommandError.persistence

                    return id |> ExerciseCategoryId.value |> CommandResult.create
            }
