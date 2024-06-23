namespace GymDiary.Persistence

open MongoDB.Bson
open MongoDB.Bson.Serialization.Conventions
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
