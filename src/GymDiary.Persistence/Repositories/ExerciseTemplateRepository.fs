namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseTemplateRepository =

    let create
        (collection: IMongoCollection<ExerciseTemplateDocument>)
        (entity: ExerciseTemplate)
        : Async<ExerciseTemplateId> =
        async {
            let! createdDocument = entity |> ExerciseTemplateDocument.fromDomain |> MongoRepository.insertOne collection

            return
                createdDocument.Id
                |> Id.create<ExerciseTemplate> (nameof createdDocument.Id)
                |> Result.valueOr (fun error ->
                    raise (DocumentConversionException(typeof<ExerciseTemplateId>.Name, error)))
        }

    let getById
        (collection: IMongoCollection<ExerciseTemplateDocument>)
        (ownerId: SportsmanId)
        (templateId: ExerciseTemplateId)
        : Async<ExerciseTemplate option> =
        async {
            let ownerId = ownerId |> Id.value
            let templateId = templateId |> Id.value

            let! documentOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = templateId && d.OwnerId = ownerId))

            return
                documentOption
                |> Option.traverseResult ExerciseTemplateDocument.toDomain
                |> Result.valueOr (fun error ->
                    raise (DocumentConversionException(typeof<ExerciseTemplateDocument>.Name, error)))
        }

    let update
        (collection: IMongoCollection<ExerciseTemplateDocument>)
        (entity: ExerciseTemplate)
        : ModifyEntityResult =
        asyncResult {
            let id = entity.Id |> Id.value

            let! result =
                entity
                |> ExerciseTemplateDocument.fromDomain
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = id))

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<ExerciseTemplate>.Name, id) |> Error
        }

    let delete
        (collection: IMongoCollection<ExerciseTemplateDocument>)
        (templateId: ExerciseTemplateId)
        : ModifyEntityResult =
        asyncResult {
            let templateId = templateId |> Id.value

            let! result = MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = templateId))

            if result.DeletedCount = 0 then
                return! EntityNotFound(typeof<ExerciseTemplate>.Name, templateId) |> Error
        }
