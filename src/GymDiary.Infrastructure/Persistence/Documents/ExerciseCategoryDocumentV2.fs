namespace GymDiary.Infrastructure.Persistence.Documents

open System
open MongoDB.Bson.Serialization.Attributes

[<CLIMutable>]
type ExerciseCategoryDocumentV2 = {
    Id: string
    Name: string
    OwnerId: string
    [<BsonIgnore>]
    mutable CreatedOnUtc: DateTime
    [<BsonIgnore>]
    mutable UpdatedOnUtc: DateTime
} with

    interface IDocumentWithOwner with
        member this.Id = this.Id
        member this.OwnerId = this.OwnerId

        [<BsonElement("CreatedOnUtc")>]
        member this.CreatedOnUtc
            with get () = this.CreatedOnUtc
            and set value = this.CreatedOnUtc <- value

        [<BsonElement("UpdatedOnUtc")>]
        member this.UpdatedOnUtc
            with get () = this.UpdatedOnUtc
            and set value = this.UpdatedOnUtc <- value
