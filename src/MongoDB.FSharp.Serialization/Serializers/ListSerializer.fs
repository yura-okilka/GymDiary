namespace MongoDB.FSharp.Serialization.Serializers

open MongoDB.Bson.Serialization.Serializers

type ListSerializer<'TItem>() =
    inherit EnumerableSerializerBase<'TItem list, 'TItem>()

    override _.CreateAccumulator() = new ResizeArray<'TItem>()

    override _.EnumerateItemsInSerializationOrder(value) = Seq.ofList value

    override _.AddItem(accumulator, item) = (accumulator :?> ResizeArray<'TItem>).Add(item)

    override _.FinalizeResult(accumulator) = accumulator :?> ResizeArray<'TItem> |> List.ofSeq
