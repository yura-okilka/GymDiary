namespace GymDiary.Persistence.Repositories

open GymDiary.Core.Extensions
open GymDiary.Core.Domain
open GymDiary.Persistence
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Conversion

open FSharpx.Collections

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseCategoryRepository =

    let private unwrapId id =
        let rawId = ExerciseCategoryId.value id
        (rawId, $"ExerciseCategory with id '%s{rawId}'")

    let create (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        asyncResult {
            let! createdDto =
                entity
                |> ExerciseCategoryDto.fromDomain
                |> MongoRepository.insertOne collection
                |> AsyncResult.mapError (PersistenceError.fromException "create ExerciseCategory")

            return!
                createdDto.Id
                |> ExerciseCategoryId.create (nameof createdDto.Id)
                |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseCategoryId>.Name)
                |> Async.singleton
        }

    let getAll (collection: IMongoCollection<ExerciseCategoryDto>) (ownerId: SportsmanId) =
        asyncResult {
            let ownerId = ownerId |> SportsmanId.value

            let! dtos =
                MongoRepository.find collection (Expr.Quote(fun d -> d.OwnerId = ownerId))
                |> AsyncResult.mapError (PersistenceError.fromException "get all ExerciseCategories")

            return!
                dtos
                |> List.traverseResultM ExerciseCategoryDto.toDomain
                |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseCategoryDto>.Name)
                |> Async.singleton
        }

    let getById
        (collection: IMongoCollection<ExerciseCategoryDto>)
        (ownerId: SportsmanId)
        (categoryId: ExerciseCategoryId)
        =
        asyncResult {
            let categoryId, entityWithIdMsg = unwrapId categoryId
            let ownerId = ownerId |> SportsmanId.value

            let! dtoOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = categoryId && d.OwnerId = ownerId))
                |> AsyncResult.mapError (PersistenceError.fromException $"get %s{entityWithIdMsg}")

            match dtoOption with
            | None -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | Some dto ->
                return!
                    dto
                    |> ExerciseCategoryDto.toDomain
                    |> Result.mapError (PersistenceError.dtoConversionFailed typeof<ExerciseCategoryDto>.Name)
                    |> Async.singleton
        }

    let existWithName (collection: IMongoCollection<ExerciseCategoryDto>) (ownerId: SportsmanId) (name: String50) =
        let name = name |> String50.value
        let ownerId = ownerId |> SportsmanId.value

        // Consider using case insensitive index for large collections.
        MongoRepository.findAny
            collection
            (Expr.Quote(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId))
        |> AsyncResult.mapError (PersistenceError.fromException $"find ExerciseCategory with name '%s{name}'")

    let update (collection: IMongoCollection<ExerciseCategoryDto>) (entity: ExerciseCategory) =
        asyncResult {
            let _, entityWithIdMsg = unwrapId entity.Id
            let dto = entity |> ExerciseCategoryDto.fromDomain

            let! result =
                dto
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = dto.Id))
                |> AsyncResult.mapError (PersistenceError.fromException $"update %s{entityWithIdMsg}")

            match result with
            | { ModifiedCount = 0L } -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | _ -> return ()
        }

    let delete (collection: IMongoCollection<ExerciseCategoryDto>) (id: ExerciseCategoryId) =
        asyncResult {
            let id, entityWithIdMsg = unwrapId id

            let! result =
                MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = id))
                |> AsyncResult.mapError (PersistenceError.fromException $"delete %s{entityWithIdMsg}")

            match result with
            | { DeletedCount = 0L } -> return! PersistenceError.entityNotFound entityWithIdMsg |> AsyncResult.error
            | _ -> return ()
        }
