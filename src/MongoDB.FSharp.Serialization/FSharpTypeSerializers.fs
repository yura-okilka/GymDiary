namespace MongoDB.FSharp.Serialization

open System

open MongoDB.Bson.Serialization
open MongoDB.FSharp.Serialization.Serializers

type FSharpTypeSerializationProvider() =

    let createInstance (objType: Type) = Activator.CreateInstance(objType)

    let asBsonSerializer (value: obj) = value :?> IBsonSerializer

    interface IBsonSerializationProvider with
        member _.GetSerializer(objType) =
            if isOption objType then
                typedefof<OptionSerializer<_>>.MakeGenericType (objType.GetGenericArguments())
                |> createInstance
                |> asBsonSerializer
            elif isList objType then
                typedefof<ListSerializer<_>>.MakeGenericType (objType.GetGenericArguments())
                |> createInstance
                |> asBsonSerializer
            else
                null

module FSharpTypeSerializers =
    let mutable private isRegistered = false

    let register () =
        if not isRegistered then
            BsonSerializer.RegisterSerializationProvider(FSharpTypeSerializationProvider())
            isRegistered <- true
