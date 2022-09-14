namespace GymDiary.Core.Domain

module ExerciseSession =

    let create exerciseId sets startedOn completedOn : ExerciseSession =
        {
            ExerciseId = exerciseId
            Sets = sets
            StartedOn = startedOn
            CompletedOn = completedOn
        }
