namespace GymDiary.Persistence

open System

open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers

module SerializationSettings =

    [<Literal>]
    let GymDiaryDBConventions = "GymDiary DB Conventions"

    let registerGlobally =
        let conventionPack = new ConventionPack()
        conventionPack.Add(new CamelCaseElementNameConvention())
        conventionPack.Add(new StringIdStoredAsObjectIdConvention())
        conventionPack.Add(new EnumRepresentationConvention(BsonType.String))

        ConventionRegistry.Register(GymDiaryDBConventions, conventionPack, (fun _ -> true))

        BsonSerializer.RegisterSerializer(typeof<char>, new CharSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<Guid>, new GuidSerializer(BsonType.String))
        BsonSerializer.RegisterSerializer(typeof<DateTimeOffset>, new DateTimeOffsetSerializer(BsonType.Document))
