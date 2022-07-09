namespace GymDiary.Persistence

open System

open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers

module SerializationSettings =

    let register () =
        let conventionPack = new ConventionPack()
        conventionPack.Add(new CamelCaseElementNameConvention())
        conventionPack.Add(new StringIdStoredAsObjectIdConvention())
        conventionPack.Add(new EnumRepresentationConvention(BsonType.String))

        ConventionRegistry.Register("GymDiary DB Conventions", conventionPack, (fun _ -> true))

        BsonSerializer.RegisterSerializer(typeof<char>, new CharSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<Guid>, new GuidSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<DateTimeOffset>, new DateTimeOffsetSerializer(BsonType.Document))
