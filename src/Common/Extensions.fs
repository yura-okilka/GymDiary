namespace Common

open System
open System.Linq.Expressions

module Extensions =

    type Expr =
        /// Creates an Expression from F# lambda. Passing function name won't work.
        /// Compiler automatically quotes a function when it is passed as an argument to a method.
        /// More info: https://stackoverflow.com/questions/23146473/how-do-i-create-a-linq-expression-tree-with-an-f-lambda
        static member Quote(e: Expression<Func<_, _>>) = e

    [<AutoOpen>]
    module Interop =

        let inline isNull value = obj.ReferenceEquals(value, null)

        let inline defaultof<'T> = Unchecked.defaultof<'T>

    module Option =

        let ofRecord value =
            match box value with
            | null -> None
            | _ -> Some value

    module Result =

        let getOk result =
            match result with
            | Ok v -> v
            | Error _ -> failwith "Result is error"

        let getError result =
            match result with
            | Ok _ -> failwith "Result is not error"
            | Error e -> e

    module DateOnly =

        let toDateTime (date: DateOnly) = date.ToDateTime(TimeOnly.MinValue)

    [<AutoOpen>]
    module UnitOfMeasure =

        open FSharp.Data.UnitSystems.SI.UnitSymbols

        let decimalM = LanguagePrimitives.DecimalWithMeasure<m>

        let decimalKg = LanguagePrimitives.DecimalWithMeasure<kg>
