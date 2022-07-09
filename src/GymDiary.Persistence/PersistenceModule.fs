namespace GymDiary.Persistence

open MongoDB.FSharp.Serialization

module PersistenceModule =

    let configure () =
        ConventionsModule.register ()
        SerializationProviderModule.register ()
        SerializationSettings.register ()
