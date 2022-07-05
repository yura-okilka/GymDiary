namespace GymDiary.Persistence

open System

open MongoDB.Bson.Serialization.Attributes

// Do not use F# types for DTOs: driver cannot deserialize most of them. Use C# analogs instead.
// It is safe to use non-nullable types for DTOs:
// driver throws "Cannot deserialize a 'DateTime' from BsonType 'Null'"
// and does not populate it silently with a default value.

[<CLIMutable>]
type RepsSetDocument = { OrderNum: int; Reps: int }

[<CLIMutable>]
type RepsWeightSetDocument =
    { OrderNum: int
      Reps: int
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationSetDocument = { OrderNum: int; Duration: TimeSpan }

[<CLIMutable>]
type DurationWeightSetDocument =
    { OrderNum: int
      Duration: TimeSpan
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationDistanceSetDocument =
    { OrderNum: int
      Duration: TimeSpan
      Distance: decimal }

// Tag to discriminate ExerciseSetsDocuments
type ExerciseSetsDocumentTag =
    | RepsSets = 1
    | RepsWeightSets = 2
    | DurationSets = 3
    | DurationWeightSets = 4
    | DurationDistanceSets = 5

[<CLIMutable>]
type ExerciseSetsDocument =
    { Tag: ExerciseSetsDocumentTag

      [<BsonIgnoreIfDefault>]
      RepsSets: ResizeArray<RepsSetDocument>

      [<BsonIgnoreIfDefault>]
      RepsWeightSets: ResizeArray<RepsWeightSetDocument>

      [<BsonIgnoreIfDefault>]
      DurationSets: ResizeArray<DurationSetDocument>

      [<BsonIgnoreIfDefault>]
      DurationWeightSets: ResizeArray<DurationWeightSetDocument>

      [<BsonIgnoreIfDefault>]
      DurationDistanceSets: ResizeArray<DurationDistanceSetDocument> }

[<CLIMutable>]
type ExerciseCategoryDocument =
    { Id: string
      Name: string
      OwnerId: string }

[<CLIMutable>]
type ExerciseTemplateDocument =
    { Id: string
      CategoryId: string
      Name: string
      Notes: string
      RestTime: TimeSpan
      Sets: ExerciseSetsDocument
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type WorkoutTemplateDocument =
    { Id: string
      Name: string
      Goal: string
      Notes: string
      Schedule: ResizeArray<DayOfWeek>
      Exercises: ResizeArray<ExerciseTemplateDocument>
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type ExerciseDocument =
    { TemplateId: string
      Sets: ExerciseSetsDocument
      StartedOn: DateTime
      CompletedOn: DateTime }

[<CLIMutable>]
type WorkoutDocument =
    { Id: string
      TemplateId: string
      Exercises: ResizeArray<ExerciseDocument>
      StartedOn: DateTime
      CompletedOn: DateTime
      OwnerId: string }

type GenderDocument =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type SportsmanDocument =
    { Id: string
      Email: string
      FirstName: string
      LastName: string
      DateOfBirth: Nullable<DateTime>
      Gender: Nullable<GenderDocument> }
