namespace GymDiary.Infrastructure.Time

open System
open GymDiary.Application.Time

type SystemClock(timeProvider: TimeProvider) =
    interface IClock with
        member _.UtcNow = timeProvider.GetUtcNow().UtcDateTime
