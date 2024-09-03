namespace GymDiary.Persistence.Documents

open System

[<CLIMutable>]
type RoutineDocument = {
    Id: string
    Name: string
    Goal: string option
    Notes: string option
    Schedule: DayOfWeek list
    ExerciseIds: string list
    CreatedOn: DateTime
    LastModifiedOn: DateTime
    OwnerId: string
}
