namespace GymDiary.Persistence

open System
open System.Linq.Expressions

open GymDiary.Core.Domain

open MongoDB.Driver

module internal InternalExtensions =

    type Expr =
        /// Creates an Expression from F# lambda. Passing function name won't work.
        /// Compiler automatically quotes a function when it is passed as an argument to a method.
        /// More info: https://stackoverflow.com/questions/23146473/how-do-i-create-a-linq-expression-tree-with-an-f-lambda
        static member Quote (e: Expression<Func<_, _>>) = e

    [<AutoOpen>]
    module Interop =

        let inline isNull value = obj.ReferenceEquals(value, null)

        let inline defaultof<'T> = Unchecked.defaultof<'T>

    [<AutoOpen>]
    module ExceptionPatterns =

        let (|ObjectIdFormatException|_|) (ex: exn) =
            match ex with
            | :? FormatException as fex when fex.Message.Contains("is not a valid 24 digit hex string") -> Some fex
            | _ -> None

    module PersistenceError =

        let fromException (operation: string) (ex: exn) =
            match ex with
            | :? MongoException -> DatabaseError(operation, ex)
            | _ -> OtherError(operation, ex)
