namespace GymDiary.Persistence

open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Persistence
open MongoDB.Bson

type IdProvider() =
    interface IIdProvider with
        member _.GenerateId<'T>() = ObjectId.GenerateNewId().ToString() |> Id.create<'T>

        member _.TryParse value =
            match ObjectId.TryParse value with
            | true, id -> id.ToString() |> Id.create |> Some
            | _ -> None
