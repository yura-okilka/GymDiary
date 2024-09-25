namespace GymDiary.Core.Workflows

open System.Threading.Tasks

type IRequestHandler<'TRequest, 'TResponse, 'TError> =
    abstract member Handle: 'TRequest -> Task<Result<'TResponse, 'TError>>
