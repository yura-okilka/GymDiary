namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseCategoryRepository =

    let private categoryWithIdMsg id = $"ExerciseCategory with id '%s{id}'"

    let create
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (entity: ExerciseCategory)
        : Async<ExerciseCategoryId> =
        async {
            let! createdDocument = entity |> ExerciseCategoryDocument.fromDomain |> MongoRepository.insertOne collection

            return
                createdDocument.Id
                |> Id.create<ExerciseCategory> (nameof createdDocument.Id)
                |> Result.valueOr (fun error ->
                    raise (DocumentConversionException(typeof<ExerciseCategoryId>.Name, error)))
        }

    let getAll
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (ownerId: SportsmanId)
        : Async<ExerciseCategory list> =
        async {
            let ownerId = ownerId |> Id.value
            let! documents = MongoRepository.find collection (Expr.Quote(fun d -> d.OwnerId = ownerId))

            return
                documents
                |> List.ofSeq
                |> List.traverseResultM ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error ->
                    raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

    let getById
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (ownerId: SportsmanId)
        (categoryId: ExerciseCategoryId)
        : Async<ExerciseCategory option> =
        async {
            let ownerId = ownerId |> Id.value
            let categoryId = categoryId |> Id.value

            let! documentOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = categoryId && d.OwnerId = ownerId))

            return
                documentOption
                |> Option.traverseResult ExerciseCategoryDocument.toDomain
                |> Result.valueOr (fun error ->
                    raise (DocumentConversionException(typeof<ExerciseCategoryDocument>.Name, error)))
        }

    let existWithName
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (ownerId: SportsmanId)
        (name: String50)
        : Async<bool> =
        let ownerId = ownerId |> Id.value
        let name = name |> String50.value

        // Consider using case insensitive index for large collections.
        MongoRepository.findAny
            collection
            (Expr.Quote(fun d -> d.Name.ToLower() = name.ToLower() && d.OwnerId = ownerId))

    let update
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (entity: ExerciseCategory)
        : ModifyEntityResult =
        asyncResult {
            let id = entity.Id |> Id.value

            let! result =
                entity
                |> ExerciseCategoryDocument.fromDomain
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = id))

            if result.ModifiedCount = 0 then
                return! Error(EntityNotFound(categoryWithIdMsg id))
        }

    let delete
        (collection: IMongoCollection<ExerciseCategoryDocument>)
        (categoryId: ExerciseCategoryId)
        : ModifyEntityResult =
        asyncResult {
            let categoryId = categoryId |> Id.value

            let! result = MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = categoryId))

            if result.DeletedCount = 0 then
                return! Error(EntityNotFound(categoryWithIdMsg categoryId))
        }
