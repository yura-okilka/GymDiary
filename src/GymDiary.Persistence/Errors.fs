namespace GymDiary.Persistence.Errors

open GymDiary.Core.Domain.Errors
open MongoDB.Driver

type PersistenceError =
    | Validation of error: ValidationError
    | Mongo of ex: MongoException
    | Other of ex: exn
