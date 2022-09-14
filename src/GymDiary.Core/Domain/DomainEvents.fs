namespace GymDiary.Core.Domain

open Microsoft.Extensions.Logging

module DomainEvents =

    let UndefinedFailure = EventId(-1, "UndefinedFailure")

    let ExerciseCategoryCreated = EventId(1, "ExerciseCategoryCreated")
    let ExerciseCategoryCreationFailed = EventId(2, "ExerciseCategoryCreationFailed")

    let ExerciseCategoryRenamed = EventId(3, "ExerciseCategoryRenamed")
    let ExerciseCategoryRenamingFailed = EventId(4, "ExerciseCategoryRenamingFailed")

    let ExerciseCategoryDeleted = EventId(5, "ExerciseCategoryDeleted")
    let ExerciseCategoryDeletionFailed = EventId(6, "ExerciseCategoryDeletionFailed")
