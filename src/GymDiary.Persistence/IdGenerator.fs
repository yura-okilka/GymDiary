namespace GymDiary.Persistence

open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Persistence
open MongoDB.Bson

type IdGenerator() =
    interface IIdGenerator with
        member _.GenerateId<'T> () = ObjectId.GenerateNewId().ToString() |> Id.create<'T>
