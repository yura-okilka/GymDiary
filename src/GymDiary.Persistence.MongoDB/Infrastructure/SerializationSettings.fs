namespace GymDiary.Persistence.MongoDB

open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers

module SerializationSettings =

    let register () =
        // Store System.Guid (incl. UMX-tagged ids) as native BSON UUID, standard representation.
        BsonSerializer.TryRegisterSerializer(GuidSerializer(GuidRepresentation.Standard)) |> ignore

        // F# records, options, lists, maps, sets and discriminated unions (FSharp.MongoDB).
        FSharp.register ()

        let conventions = ConventionPack()
        conventions.Add(CamelCaseElementNameConvention())
        conventions.Add(EnumRepresentationConvention(BsonType.String))
        conventions.Add(IgnoreIfNullConvention(true)) // Filtering with $exists has a better performance

        ConventionRegistry.Register("GymDiary Conventions", conventions, (fun _ -> true))
