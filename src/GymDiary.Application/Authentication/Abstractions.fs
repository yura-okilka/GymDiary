namespace GymDiary.Application.Authentication

type IUserContext =
    abstract member IsAuthenticated: bool
    abstract member UserId: string
    abstract member Email: string
