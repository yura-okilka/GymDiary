namespace MongoDB.FSharp.Serialization

open Microsoft.FSharp.Reflection

[<AutoOpen>]
module internal TypeUtils =

    let isOption objType =
        FSharpType.IsUnion objType
        && objType.IsGenericType
        && objType.GetGenericTypeDefinition() = typedefof<_ option>

    let isList objType =
        FSharpType.IsUnion objType
        && objType.IsGenericType
        && objType.GetGenericTypeDefinition() = typedefof<_ list>

    let getUnionCases objType =
        FSharpType.GetUnionCases(objType) |> Seq.map (fun x -> (x.Name, x)) |> dict
