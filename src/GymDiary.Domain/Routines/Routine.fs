namespace GymDiary.Domain.Routines

open System
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Users

/// A template of a workout
type Routine = {
    Id: Id<Routine>
    Name: String50
    Goal: String200 option
    Notes: String1k option
    Schedule: DayOfWeek Set
    Exercises: Id<ExerciseDefinition> Set
    OwnerId: Id<User>
}

type RoutineId = Id<Routine>

module Routine =
    let create id name goal notes schedule exercises ownerId : Result<Routine, string> =
        if Set.isEmpty exercises then
            Error "Routine must have at least one exercise"
        else
            Ok {
                Id = id
                Name = name
                Goal = goal
                Notes = notes
                Schedule = schedule
                Exercises = exercises
                OwnerId = ownerId
            }

    let update name goal notes schedule exercises routine = {
        routine with
            Name = name
            Goal = goal
            Notes = notes
            Schedule = schedule
            Exercises = exercises
    }
