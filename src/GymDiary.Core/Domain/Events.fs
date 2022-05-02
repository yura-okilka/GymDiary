namespace GymDiary.Core.Domain

open Microsoft.Extensions.Logging

module Events =

    let UndefinedFailure = EventId(-1, "UndefinedFailure")

    let ExerciseCategoryCreated = EventId(1, "ExerciseCategoryCreated")
    let ExerciseCategoryCreationFailed = EventId(2, "ExerciseCategoryCreationFailed")

    let ExerciseCategoryRenamed = EventId(3, "ExerciseCategoryRenamed")
    let ExerciseCategoryRenamingFailed = EventId(4, "ExerciseCategoryRenamingFailed")
