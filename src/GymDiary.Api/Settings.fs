namespace GymDiary.Api

[<CLIMutable>]
type MongoDbSettings =
    { ConnectionString: string
      DatabaseName: string }

[<CLIMutable>]
type Settings =
    { MongoDb: MongoDbSettings }
