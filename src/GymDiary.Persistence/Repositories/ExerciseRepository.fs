namespace GymDiary.Persistence.Repositories

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Core.Persistence
open GymDiary.Persistence
open GymDiary.Persistence.Conversion

open FsToolkit.ErrorHandling

open MongoDB.Driver

module ExerciseRepository =

    let create (collection: IMongoCollection<ExerciseDocument>) (entity: Exercise) : Async<ExerciseId> =
        async {
            let! createdDocument = entity |> ExerciseDocument.fromDomain |> MongoRepository.insertOne collection

            return
                createdDocument.Id
                |> Id.create<Exercise> (nameof createdDocument.Id)
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseId>.Name, error)))
        }

    let getById
        (collection: IMongoCollection<ExerciseDocument>)
        (ownerId: SportsmanId)
        (exerciseId: ExerciseId)
        : Async<Exercise option> =
        async {
            let ownerId = ownerId |> Id.value
            let exerciseId = exerciseId |> Id.value

            let! documentOption =
                MongoRepository.findSingle collection (Expr.Quote(fun d -> d.Id = exerciseId && d.OwnerId = ownerId))

            return
                documentOption
                |> Option.traverseResult ExerciseDocument.toDomain
                |> Result.valueOr (fun error -> raise (DocumentConversionException(typeof<ExerciseDocument>.Name, error)))
        }

    let update (collection: IMongoCollection<ExerciseDocument>) (entity: Exercise) : ModifyEntityResult =
        asyncResult {
            let id = entity.Id |> Id.value

            let! result =
                entity
                |> ExerciseDocument.fromDomain
                |> MongoRepository.replaceOne collection (Expr.Quote(fun d -> d.Id = id))

            if result.ModifiedCount = 0 then
                return! EntityNotFound(typeof<Exercise>.Name, id) |> Error
        }

    let delete (collection: IMongoCollection<ExerciseDocument>) (exerciseId: ExerciseId) : ModifyEntityResult =
        asyncResult {
            let exerciseId = exerciseId |> Id.value

            let! result = MongoRepository.deleteOne collection (Expr.Quote(fun d -> d.Id = exerciseId))

            if result.DeletedCount = 0 then
                return! EntityNotFound(typeof<Exercise>.Name, exerciseId) |> Error
        }
