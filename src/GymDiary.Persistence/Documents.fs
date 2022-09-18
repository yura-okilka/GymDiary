namespace GymDiary.Persistence

open System

open MongoDB.Bson.Serialization.Attributes

// It is safe to use non-nullable types for DTOs:
// driver throws "Cannot deserialize a 'DateTime' from BsonType 'Null'"
// and does not populate it silently with a default value.

[<AutoOpen>]
module Documents =

    [<CLIMutable>]
    type SportsmanDocument =
        {
            Id: string
            Email: string
            FirstName: string
            LastName: string
            DateOfBirth: DateTime option
            Gender: string option
        }

    [<CLIMutable>]
    type ExerciseCategoryDocument =
        {
            Id: string
            Name: string
            OwnerId: string
        }

    type ExerciseSetType =
        | RepsSet = 1
        | RepsWeightSet = 2
        | DurationSet = 3
        | DurationWeightSet = 4
        | DurationDistanceSet = 5

    /// A superset of all exercise set types.
    [<CLIMutable>]
    type ExerciseSetDocument =
        {
            OrderNum: int

            [<BsonIgnoreIfDefault>]
            Reps: int option

            [<BsonIgnoreIfDefault>]
            EquipmentWeight: float option

            [<BsonIgnoreIfDefault>]
            Duration: TimeSpan option

            [<BsonIgnoreIfDefault>]
            Distance: float option
        }

    [<CLIMutable>]
    type ExerciseDocument =
        {
            Id: string
            CategoryId: string
            Name: string
            Notes: string option
            RestTime: TimeSpan
            SetsType: ExerciseSetType
            Sets: ExerciseSetDocument list
            CreatedOn: DateTime
            LastModifiedOn: DateTime
            OwnerId: string
        }

    [<CLIMutable>]
    type RoutineDocument =
        {
            Id: string
            Name: string
            Goal: string option
            Notes: string option
            Schedule: DayOfWeek list
            Exercises: ExerciseDocument list
            CreatedOn: DateTime
            LastModifiedOn: DateTime
            OwnerId: string
        }

    [<CLIMutable>]
    type ExerciseSessionDocument =
        {
            ExerciseId: string
            SetsType: ExerciseSetType
            Sets: ExerciseSetDocument list
            StartedOn: DateTime
            CompletedOn: DateTime
        }

    [<CLIMutable>]
    type WorkoutSessionDocument =
        {
            Id: string
            RoutineId: string
            Exercises: ExerciseSessionDocument list
            StartedOn: DateTime
            CompletedOn: DateTime
            OwnerId: string
        }
