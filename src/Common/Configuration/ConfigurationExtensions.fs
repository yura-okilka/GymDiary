namespace Common.Configuration

open System
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration

type ConfigurationExtensions() =

    [<Extension>]
    static member GetOrThrow(configuration: IConfiguration) : 'T =
        match configuration.Get<'T>() with
        | null -> raise (InvalidOperationException($"Cannot bind configuration to type {typeof<'T>.Name}"))
        | value -> value

    [<Extension>]
    static member GetValueOrThrow(configuration: IConfiguration, name: string) : 'T =
        match configuration.GetValue<'T>(name) with
        | null -> raise (InvalidOperationException($"Value {name} was not found"))
        | value -> value

    [<Extension>]
    static member GetConnectionStringOrThrow(configuration: IConfiguration, name: string) : string =
        match configuration.GetConnectionString(name) with
        | null -> raise (InvalidOperationException($"Connection string {name} was not found"))
        | value -> value
