namespace GymDiary.Persistence.Repositories

open System
open System.Linq.Expressions
open Common.Extensions
open GymDiary.Persistence
open MongoDB.Driver

type Filter<'TDocument> = Expression<Func<'TDocument, bool>>

/// MongoDB repository to work with data in F# types.
type IMongoRepository<'TDocument> =
    abstract member FindAll: Filter<'TDocument> -> Async<'TDocument seq>
    abstract member FindSingle: Filter<'TDocument> -> Async<'TDocument option>
    abstract member Any: Filter<'TDocument> -> Async<bool>
    abstract member InsertOne: 'TDocument -> Async<'TDocument>
    abstract member ReplaceOne: Filter<'TDocument> -> 'TDocument -> Async<ReplaceOneResult>
    abstract member DeleteOne: Filter<'TDocument> -> Async<DeleteResult>

type MongoRepository<'TDocument>(mongoClient: IMongoClient, mongoSettings: MongoSettings, collection: string) =
    member private _.GetCollection() =
        mongoClient
            .GetDatabase(mongoSettings.Database)
            .GetCollection<'TDocument>(collection)

    interface IMongoRepository<'TDocument> with
        member r.FindAll filter =
            task {
                let! documents = r.GetCollection().Find(filter).ToListAsync()
                return documents :> seq<_>
            }
            |> Async.AwaitTask

        member r.FindSingle filter =
            task {
                let! document = r.GetCollection().Find(filter).SingleOrDefaultAsync()
                return Option.ofRecord document
            }
            |> Async.AwaitTask

        member r.Any filter =
            r.GetCollection().Find(filter).AnyAsync() |> Async.AwaitTask

        member r.InsertOne document =
            task {
                do! r.GetCollection().InsertOneAsync(document)
                return document
            }
            |> Async.AwaitTask

        member r.ReplaceOne filter document =
            r.GetCollection().ReplaceOneAsync(filter, document) |> Async.AwaitTask

        member r.DeleteOne filter = r.GetCollection().DeleteOneAsync(filter) |> Async.AwaitTask
