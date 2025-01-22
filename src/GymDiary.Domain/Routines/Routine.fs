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
} with

    static member create id name goal notes schedule exercises ownerId : Routine = {
        Id = id
        Name = name
        Goal = goal
        Notes = notes
        Schedule = schedule
        Exercises = exercises
        OwnerId = ownerId
    }

    static member update name goal notes schedule exercises routine : Routine = {
        routine with
            Name = name
            Goal = goal
            Notes = notes
            Schedule = schedule
            Exercises = exercises
    }

type RoutineId = Id<Routine>
