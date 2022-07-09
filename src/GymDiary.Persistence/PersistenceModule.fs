namespace GymDiary.Persistence

open MongoDB.FSharp.Serialization

module PersistenceModule =

    let configure () =
        FSharpTypeConventions.register ()
        FSharpTypeSerializers.register ()
        SerializationSettings.register ()
