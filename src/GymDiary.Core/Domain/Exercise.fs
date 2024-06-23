namespace GymDiary.Core.Domain

module Exercise =

    let create exerciseId sets startedOn completedOn : Exercise = {
        ExerciseDefinitionId = exerciseId
        Sets = sets
        StartedOn = startedOn
        CompletedOn = completedOn
    }
