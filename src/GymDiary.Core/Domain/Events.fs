namespace GymDiary.Core.Domain

open Microsoft.Extensions.Logging

module Events =

    let undefinedFailure = EventId(-1, "UndefinedFailure")
