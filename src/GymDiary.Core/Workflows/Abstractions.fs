namespace GymDiary.Core.Workflows

type Workflow<'Request, 'Response, 'Error> = 'Request -> Async<Result<'Response, 'Error>>
