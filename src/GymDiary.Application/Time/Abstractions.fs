namespace GymDiary.Application.Time

open System

type IClock =
    abstract member UtcNow: DateTime
