namespace GymDiary.Infrastructure.Persistence

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Application.Persistence
open MongoDB.Bson

type MongoObjectIdProvider() =
    member _.GenerateId() = Id(ObjectId.GenerateNewId().ToString())

    member _.TryParse value =
        match ObjectId.TryParse value with
        | true, id -> Some(Id(id.ToString()))
        | _ -> None

    member p.TryParseResult value =
        match p.TryParse value with
        | None -> Error $"{value} is not a valid {nameof ObjectId}"
        | Some id -> Ok id

    interface IEntityIdProvider with
        member p.GenerateId() = p.GenerateId()
        member p.TryParse value = p.TryParse value
        member p.TryParseResult value = p.TryParseResult value
