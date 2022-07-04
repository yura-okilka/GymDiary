namespace GymDiary.Persistence.Repositories

open System
open System.Linq.Expressions

open Common.Extensions

open MongoDB.Driver

/// MongoDB repository to work with data in F# types.
module MongoRepository =

    let find (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            let! documents = collection.Find(filter).ToListAsync()

            return documents :> seq<_>
        }
        |> Async.AwaitTask

    let findSingle (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        task {
            let! document = collection.Find(filter).SingleOrDefaultAsync()

            return Option.ofRecord document
        }
        |> Async.AwaitTask

    let findAny (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        collection.Find(filter).AnyAsync() |> Async.AwaitTask

    let insertOne (collection: IMongoCollection<'Document>) (document: 'Document) =
        task {
            do! collection.InsertOneAsync(document)

            return document
        }
        |> Async.AwaitTask

    let replaceOne
        (collection: IMongoCollection<'Document>)
        (filter: Expression<Func<'Document, bool>>)
        (document: 'Document)
        =
        collection.ReplaceOneAsync(filter, document) |> Async.AwaitTask

    let deleteOne (collection: IMongoCollection<'Document>) (filter: Expression<Func<'Document, bool>>) =
        collection.DeleteOneAsync(filter) |> Async.AwaitTask
