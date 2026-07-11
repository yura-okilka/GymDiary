namespace GymDiary.Application.Workflows

open System.Threading.Tasks

type public IRequestHandler<'TRequest, 'TResponse, 'TError> =
    abstract member Handle: 'TRequest -> Task<Result<'TResponse, 'TError>>
