namespace GymDiary.Core.Workflows

type IRequestHandler<'TRequest, 'TResponse, 'TError> =
    abstract member Handle: 'TRequest -> Async<Result<'TResponse, 'TError>>
