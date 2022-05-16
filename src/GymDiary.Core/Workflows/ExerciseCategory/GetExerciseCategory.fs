namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetExerciseCategory =

    type Query = { Id: string }

    type QueryResult =
        { Id: string
          Name: string
          OwnerId: string }

    type QueryError =
        | Domain of DomainError
        | Persistence of PersistenceError

        static member domain e = Domain e
        static member persistence e = Persistence e

    type Workflow = Workflow<Query, QueryResult, QueryError>

    let createWorkflow
        (getCategoryByIdFromDB: ExerciseCategoryId -> Async<Result<ExerciseCategory, PersistenceError>>)
        : Workflow =
        fun query ->
            asyncResult {
                let! id =
                    ExerciseCategoryId.create (nameof query.Id) query.Id
                    |> Result.setError (ExerciseCategoryNotFound |> QueryError.domain)

                let! category =
                    getCategoryByIdFromDB id
                    |> AsyncResult.mapError (fun error ->
                        match error with
                        | EntityNotFound _ -> ExerciseCategoryNotFound |> QueryError.domain
                        | _ -> error |> QueryError.persistence)

                return
                    { Id = category.Id |> ExerciseCategoryId.value
                      Name = category.Name |> String50.value
                      OwnerId = category.OwnerId |> SportsmanId.value }
            }
