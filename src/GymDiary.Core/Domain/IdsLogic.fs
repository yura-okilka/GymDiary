namespace GymDiary.Core.Domain.Logic

open System

open GymDiary.Core.Domain

module ExerciseCategoryId =

    let Empty = ExerciseCategoryId ""

    let create fieldName value =
        ConstrainedType.createString fieldName ExerciseCategoryId (0, Int32.MaxValue) value

    let value (ExerciseCategoryId id) = id

module ExerciseTemplateId =

    let Empty = ExerciseTemplateId ""

    let create fieldName value =
        ConstrainedType.createString fieldName ExerciseTemplateId (0, Int32.MaxValue) value

    let value (ExerciseTemplateId id) = id

module WorkoutTemplateId =

    let Empty = WorkoutTemplateId ""

    let create fieldName value =
        ConstrainedType.createString fieldName WorkoutTemplateId (0, Int32.MaxValue) value

    let value (WorkoutTemplateId id) = id

module WorkoutId =

    let Empty = WorkoutId ""

    let create fieldName value =
        ConstrainedType.createString fieldName WorkoutId (0, Int32.MaxValue) value

    let value (WorkoutId id) = id

module SportsmanId =

    let Empty = SportsmanId ""

    let create fieldName value =
        ConstrainedType.createString fieldName SportsmanId (0, Int32.MaxValue) value

    let value (SportsmanId id) = id
