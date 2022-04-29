namespace GymDiary.Persistence

module PersistenceModule =

    let configure () =
        // Configure module
        SerializationSettings.registerGlobally ()
