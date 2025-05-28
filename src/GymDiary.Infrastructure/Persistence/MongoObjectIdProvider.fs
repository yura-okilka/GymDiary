namespace GymDiary.Infrastructure.Persistence

open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Application.Persistence
open MongoDB.Bson

type MongoObjectIdProvider() =
    member _.GenerateId() = ObjectId.GenerateNewId().ToString() |> Id

    member _.TryParse value =
        match ObjectId.TryParse value with
        | true, id -> id.ToString() |> Id |> Some
        | _ -> None

    member this.TryParseResult value =
        match this.TryParse value with
        | None -> Error $"{value} is not a valid {nameof ObjectId}" // TODO: check
        | Some id -> Ok id

    interface IEntityIdProvider with
        member this.GenerateId() = this.GenerateId()
        member this.TryParse value = this.TryParse value
        member this.TryParseResult(value) = this.TryParseResult value
