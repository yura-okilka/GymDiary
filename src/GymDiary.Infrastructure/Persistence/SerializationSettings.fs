namespace GymDiary.Infrastructure.Persistence

open MongoDB.Bson
open MongoDB.Bson.Serialization.Conventions
open MongoDB.FSharp.Serialization

module SerializationSettings =

    let register () =
        FSharpTypeConventions.register ()
        FSharpTypeSerializers.register ()

        let conventions = ConventionPack()
        conventions.Add(CamelCaseElementNameConvention())
        conventions.Add(StringIdStoredAsObjectIdConvention())
        conventions.Add(EnumRepresentationConvention(BsonType.String))
        conventions.Add(IgnoreIfNullConvention(true)) // Filtering with $exists has a better performance

        ConventionRegistry.Register("GymDiary Conventions", conventions, (fun _ -> true))
