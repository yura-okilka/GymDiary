namespace GymDiary.Persistence

open System
open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers
open MongoDB.FSharp.Serialization

module SerializationSettings =

    let register () =
        FSharpTypeConventions.register ()
        FSharpTypeSerializers.register ()

        let conventionPack = ConventionPack()
        conventionPack.Add(CamelCaseElementNameConvention())
        conventionPack.Add(StringIdStoredAsObjectIdConvention())
        conventionPack.Add(EnumRepresentationConvention(BsonType.String))

        ConventionRegistry.Register("GymDiary DB Conventions", conventionPack, (fun _ -> true))
        // TODO: delete unused
        BsonSerializer.RegisterSerializer(typeof<char>, CharSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<Guid>, GuidSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<DateTimeOffset>, DateTimeOffsetSerializer(BsonType.Document))
