namespace GymDiary.Infrastructure.Persistence.Documents

open System
open MongoDB.Bson.Serialization.Attributes

type GenderDtoV2 =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type UserDocumentV2 = {
    Id: string
    Email: string
    FirstName: string
    LastName: string
    DateOfBirth: DateTime option
    Gender: GenderDtoV2 option
    mutable CreatedOnUtc: DateTime
    mutable UpdatedOnUtc: DateTime
} with

    interface IDocument with
        member this.Id = this.Id

        [<BsonIgnore>]
        member this.CreatedOnUtc
            with get () = this.CreatedOnUtc
            and set value = this.CreatedOnUtc <- value

        [<BsonIgnore>]
        member this.UpdatedOnUtc
            with get () = this.UpdatedOnUtc
            and set value = this.UpdatedOnUtc <- value
