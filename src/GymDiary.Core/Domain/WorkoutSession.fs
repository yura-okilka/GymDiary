namespace GymDiary.Core.Domain

module WorkoutSession =

    let create id routineId exercises startedOn completedOn ownerId : WorkoutSession =
        {
            Id = id
            RoutineId = routineId
            Exercises = exercises
            StartedOn = startedOn
            CompletedOn = completedOn
            OwnerId = ownerId
        }
