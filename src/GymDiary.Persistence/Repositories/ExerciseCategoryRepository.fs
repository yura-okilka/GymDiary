namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Conversion

open FSharpx.Collections

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseCategoryRepository =

    let private categoryWithIdMsg id = $"ExerciseCategory with id '%s{id}'"

    let create (collection: IMongoCollection<ExerciseCategoryDocument>) (entity: ExerciseCategory) =
        asyncResult {
            let! createdDocument =
                entity
                |> ExerciseCategoryDocument.fromDomain
                |> MongoRepository.insertOne collection
                |> AsyncResult.mapError (PersistenceError.fromException "create ExerciseCategory")

            return!
                createdDocument.Id
                |> Id.create (nameof createdDocument.Id)
                |> Result.mapError (PersistenceError.dtoConversionFailed typeof<Id<ExerciseCategory>>.Name)
                |> Async.singleton
        }

    let getAll (collection: IMongoCollection<ExerciseCategoryDocument>) (ownerId: Id<Sportsman>) =
        asyncResult {
            let ownerId = ownerId |> Id.value

            let! dtos =
                MongoRepository.find collection (Expr.Quote(fun d -> d.OwnerId = ownerId))
                |> AsyncResult.mapError (PersistenceError.fromException "get all ExerciseCategories")

            return!
                dtos
                |> List.traverseResultM ExerciseCategoryDocument.toDomain
                |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseCategoryDocument>.Name)
                |> Async.singleton
        }

    let getById
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (ownerId: Id<Sportsman>)
        (categoryId: Id<ExerciseCategory>)
        =
        asyncResult {
            let ownerId = ownerId |> Id.value
            let categoryId = categoryId |> Id.value
            let entityWithIdMsg = categoryWithIdMsg categoryId

            let! dtoOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = categoryId && d.OwnerId = ownerId))
                |> AsyncResult.mapError (PersistenceError.fromException $"get %s{entityWithIdMsg}")

            match dtoOption with
            | None -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | Some dto ->
                return!
                    dto
                    |> ExerciseCategoryDocument.toDomain
                    |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseCategoryDocument>.Name)
                    |> Async.singleton
        }

    let existWithName (collection: IMongoCollection<ExerciseCategoryDocument>) (ownerId: Id<Sportsman>) (name: String50) =
        let name = name |> String50.value
        let ownerId = ownerId |> Id.value

        // Consider using case insensitive index for large collections.
        MongoRepository.findAny
            collection
            (Expr.Quote(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId))
        |> AsyncResult.mapError (PersistenceError.fromException $"find ExerciseCategory with name '%s{name}'")

    let update (collection: IMongoCollection<ExerciseCategoryDocument>) (entity: ExerciseCategory) =
        asyncResult {
            let entityWithIdMsg = categoryWithIdMsg (entity.Id |> Id.value)
            let dto = entity |> ExerciseCategoryDocument.fromDomain

            let! result =
                dto
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = dto.Id))
                |> AsyncResult.mapError (PersistenceError.fromException $"update %s{entityWithIdMsg}")

            if result.ModifiedCount = 0L then
                return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
        }

    let delete (collection: IMongoCollection<ExerciseCategoryDocument>) (categoryId: Id<ExerciseCategory>) =
        asyncResult {
            let categoryId = categoryId |> Id.value
            let entityWithIdMsg = categoryWithIdMsg categoryId

            let! result =
                MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = categoryId))
                |> AsyncResult.mapError (PersistenceError.fromException $"delete %s{entityWithIdMsg}")

            if result.DeletedCount = 0L then
                return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
        }
