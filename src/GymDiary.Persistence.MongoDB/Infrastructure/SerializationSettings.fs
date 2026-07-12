namespace GymDiary.Persistence.MongoDB

open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Conventions
open MongoDB.Bson.Serialization.Serializers

module SerializationSettings =

    // Global MongoDB driver state: serializers and conventions take effect only before a type's class
    // map freezes (on first serialization). Guarded by a lazy so repeated calls (app startup, each test
    // host) are safe and no caller needs its own guard. Must run before any Mongo client is used.
    let private registered =
        lazy
            // Store System.Guid (incl. UMX-tagged ids) as native BSON UUID, standard representation.
            BsonSerializer.TryRegisterSerializer(GuidSerializer(GuidRepresentation.Standard)) |> ignore

            // F# records, options, lists, maps, sets and discriminated unions (FSharp.MongoDB).
            FSharp.register ()

            let conventions = ConventionPack()
            conventions.Add(CamelCaseElementNameConvention())
            conventions.Add(EnumRepresentationConvention(BsonType.String))
            // Omit null/None fields from documents. Deliberate: erases the null-vs-absent distinction on
            // read (fine for F# `option`), and filtering with $exists has a better performance than $eq: null.
            conventions.Add(IgnoreIfNullConvention(true))

            // Scope conventions to our own types so we never reshape driver-internal or third-party class maps.
            ConventionRegistry.Register(
                "GymDiary Conventions",
                conventions,
                fun t -> t.FullName.StartsWith("GymDiary"))

    let register () = registered.Force()
