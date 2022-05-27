namespace GymDiary.Core.Workflows

open GymDiary.Core.Domain

type Workflow<'Request, 'Response, 'Error> = 'Request -> Async<Result<'Response, 'Error>>

type PersistenceResult<'Value> = Async<Result<'Value, PersistenceError>>
