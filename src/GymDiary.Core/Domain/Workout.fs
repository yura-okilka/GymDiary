namespace GymDiary.Core.Domain

module Workout =

    let create id routineId exercises startedOn completedOn ownerId : Workout = {
        Id = id
        RoutineId = routineId
        Exercises = exercises
        StartedOn = startedOn
        CompletedOn = completedOn
        OwnerId = ownerId
    }
