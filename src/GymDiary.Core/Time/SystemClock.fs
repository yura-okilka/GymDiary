namespace GymDiary.Core.Time

open System

type IClock =
    abstract member UtcNow: DateTime

type SystemClock(timeProvider: TimeProvider) =
    interface IClock with
        member _.UtcNow = timeProvider.GetUtcNow().UtcDateTime
