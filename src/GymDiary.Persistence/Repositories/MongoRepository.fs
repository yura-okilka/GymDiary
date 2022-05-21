namespace GymDiary.Persistence.Repositories

open System
open System.Linq.Expressions

open GymDiary.Core.Extensions
open GymDiary.Persistence.InternalExtensions

open FSharpx.Collections

open MongoDB.Driver

/// MongoDB repository to access data in a safe way.
/// Returns data in F# types.
module MongoRepository =

    type ReplaceResult = { ModifiedCount: int64 }

    type DeleteResult = { DeletedCount: int64 }

    let find (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            try
                let! documents = collection.Find(filter).ToListAsync()

                return documents |> ResizeArray.toList |> Ok
            with
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask

    let findSingle (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            try
                let! document = collection.Find(filter).SingleOrDefaultAsync()

                return document |> Option.ofRecord |> Ok
            with
            | ObjectIdFormatException _ -> return None |> Ok
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask

    let findAny (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            try
                let! exists = collection.Find(filter).AnyAsync()

                return exists |> Ok
            with
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask

    let insertOne (collection: IMongoCollection<'Document>) (document: 'Document) =
        task {
            try
                do! collection.InsertOneAsync(document)

                return document |> Ok
            with
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask

    let replaceOne
        (collection: IMongoCollection<'Document>)
        (filter: Expression<Func<'Document, bool>>)
        (document: 'Document)
        =
        task {
            try
                let! result = collection.ReplaceOneAsync(filter, document)

                return { ModifiedCount = result.ModifiedCount } |> Ok
            with
            | ObjectIdFormatException _ -> return { ModifiedCount = 0 } |> Ok
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask

    let deleteOne (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            try
                let! result = collection.DeleteOneAsync(filter)

                return { DeletedCount = result.DeletedCount } |> Ok
            with
            | ObjectIdFormatException _ -> return { DeletedCount = 0 } |> Ok
            | ex -> return ex |> Error
        }
        |> Async.AwaitTask
