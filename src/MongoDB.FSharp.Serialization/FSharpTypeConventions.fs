namespace MongoDB.FSharp.Serialization

open MongoDB.Bson.Serialization.Conventions

type OptionConvention() =
    inherit ConventionBase("F# Option Type")

    interface IMemberMapConvention with
        member _.Apply (memberMap) =
            let objType = memberMap.MemberType

            if isOption objType then
                memberMap.SetDefaultValue None |> ignore
                memberMap.SetIgnoreIfNull true |> ignore

module ConventionsModule =
    let mutable private isRegistered = false

    let register () =
        if not isRegistered then
            let conventionPack = ConventionPack()
            conventionPack.Add(OptionConvention())
            ConventionRegistry.Register("F# Type Conventions", conventionPack, (fun _ -> true))
            isRegistered <- true
