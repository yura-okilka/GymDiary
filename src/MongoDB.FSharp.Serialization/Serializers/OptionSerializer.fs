namespace MongoDB.FSharp.Serialization.Serializers

open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Serializers
open MongoDB.FSharp.Serialization

open Microsoft.FSharp.Reflection

type OptionSerializer<'T>() =
    inherit SerializerBase<'T option>()

    let Cases = getUnionCases typeof<'T option>

    override _.Serialize (context, args, value) =
        match value with
        | Some x -> BsonSerializer.Serialize(context.Writer, x)
        | None -> BsonSerializer.Serialize(context.Writer, null)

    override _.Deserialize (context, args) =
        let genericTypeArgument = typeof<'T>

        let (unionCase, unionArgs) =
            let value =
                if (genericTypeArgument.IsPrimitive) then
                    BsonSerializer.Deserialize(context.Reader, typeof<obj>)
                else
                    BsonSerializer.Deserialize(context.Reader, genericTypeArgument)

            match value with
            | null -> (Cases.["None"], [||])
            | _ -> (Cases.["Some"], [| value |])

        FSharpValue.MakeUnion(unionCase, unionArgs) :?> 'T option
