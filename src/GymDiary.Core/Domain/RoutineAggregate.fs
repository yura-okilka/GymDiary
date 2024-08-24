module GymDiary.Core.Domain.RoutineAggregate

open System
open GymDiary.Core.Domain.ExerciseDefinitionAggregate
open GymDiary.Core.Domain.UserAggregate

/// A template of a workout
type Routine = {
    Id: Id<Routine>
    Name: String50
    Goal: String200 option
    Notes: String1k option
    Schedule: DayOfWeek Set
    Exercises: Id<ExerciseDefinition> Set
    CreatedOn: DateTime
    LastModifiedOn: DateTime
    OwnerId: Id<User>
}

type RoutineId = Id<Routine>

let create id name goal notes schedule exercises createdOn lastModifiedOn ownerId : Routine = {
    Id = id
    Name = name
    Goal = goal
    Notes = notes
    Schedule = schedule
    Exercises = exercises
    CreatedOn = createdOn
    LastModifiedOn = lastModifiedOn
    OwnerId = ownerId
}
