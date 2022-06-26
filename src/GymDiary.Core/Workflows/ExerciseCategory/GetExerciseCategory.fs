namespace GymDiary.Core.Workflows.ExerciseCategory

open GymDiary.Core.Domain
open GymDiary.Core.Workflows

open FsToolkit.ErrorHandling

module GetExerciseCategory =

    type Query = { Id: string; OwnerId: string }

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

    let runWorkflow
        (getCategoryByIdFromDB: Id<Sportsman> -> Id<ExerciseCategory> -> PersistenceResult<ExerciseCategory>)
        (query: Query)
        =
        asyncResult {
            let! (categoryId, ownerId) =
                result {
                    let! categoryId = Id.create (nameof query.Id) query.Id
                    let! ownerId = Id.create (nameof query.OwnerId) query.OwnerId
                    return (categoryId, ownerId)
                }
                |> Result.setError (ExerciseCategoryNotFound |> QueryError.domain)

            let! category =
                getCategoryByIdFromDB ownerId categoryId
                |> AsyncResult.mapError (fun error ->
                    match error with
                    | EntityNotFound _ -> ExerciseCategoryNotFound |> QueryError.domain
                    | _ -> error |> QueryError.persistence)

            return
                { Id = category.Id |> Id.value
                  Name = category.Name |> String50.value
                  OwnerId = category.OwnerId |> Id.value }
        }
