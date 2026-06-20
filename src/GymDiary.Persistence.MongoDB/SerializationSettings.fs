namespace GymDiary.Persistence.MongoDB

open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers
open MongoDB.FSharp.Serialization

module SerializationSettings =

    let register () =
        // Store System.Guid (incl. UMX-tagged ids) as native BSON UUID, standard representation.
        BsonSerializer.TryRegisterSerializer(GuidSerializer(GuidRepresentation.Standard)) |> ignore

        FSharpTypeConventions.register ()
        FSharpTypeSerializers.register ()

        let conventions = ConventionPack()
        conventions.Add(CamelCaseElementNameConvention())
        conventions.Add(EnumRepresentationConvention(BsonType.String))
        conventions.Add(IgnoreIfNullConvention(true)) // Filtering with $exists has a better performance

        ConventionRegistry.Register("GymDiary Conventions", conventions, (fun _ -> true))
