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
    CreatedOnUtc: DateTime
    UpdatedOnUtc: DateTime
}

type RoutineId = Id<Routine>

module Routine =
    let private validateExercises exercises =
        if Set.isEmpty exercises then
            Error "Routine must have at least one exercise"
        else
            Ok()

    let create id name goal notes schedule exercises ownerId utcNow : Result<Routine, string> =
        match validateExercises exercises with
        | Error message -> Error message
        | Ok() ->
            Ok {
                Id = id
                Name = name
                Goal = goal
                Notes = notes
                Schedule = schedule
                Exercises = exercises
                OwnerId = ownerId
                CreatedOnUtc = utcNow
                UpdatedOnUtc = utcNow
            }

    let update name goal notes schedule exercises routine utcNow : Result<Routine, string> =
        match validateExercises exercises with
        | Error message -> Error message
        | Ok() ->
            Ok {
                routine with
                    Name = name
                    Goal = goal
                    Notes = notes
                    Schedule = schedule
                    Exercises = exercises
                    UpdatedOnUtc = utcNow
            }
