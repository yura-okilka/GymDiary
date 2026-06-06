namespace GymDiary.Infrastructure.Persistence

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Application.Persistence
open MongoDB.Bson

type MongoObjectIdFactory() =
    member _.GenerateId() = Id(ObjectId.GenerateNewId().ToString())

    member _.TryParseResult value =
        match ObjectId.TryParse value with
        | true, id -> Ok(Id(id.ToString()))
        | _ -> Error $"{value} is not a valid {nameof ObjectId}"

    interface IEntityIdFactory with
        member p.GenerateId() = p.GenerateId()
        member p.TryParseResult value = p.TryParseResult value
