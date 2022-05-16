namespace GymDiary.Persistence

open System

open MongoDB.Bson.Serialization.Attributes

// Do not use F# types for DTOs: driver cannot deserialize most of them. Use C# analogs instead.
// It is safe to use non-nullable types for DTOs:
// driver throws "Cannot deserialize a 'DateTime' from BsonType 'Null'"
// and does not populate it silently with a default value.

[<CLIMutable>]
type RepsSetDto =
    { OrderNum: int
      Reps: int }

[<CLIMutable>]
type RepsWeightSetDto =
    { OrderNum: int
      Reps: int
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationSetDto =
    { OrderNum: int
      Duration: TimeSpan }

[<CLIMutable>]
type DurationWeightSetDto =
    { OrderNum: int
      Duration: TimeSpan
      EquipmentWeight: decimal }

[<CLIMutable>]
type DurationDistanceSetDto =
    { OrderNum: int
      Duration: TimeSpan
      Distance: decimal }

// Tag to discriminate ExerciseSetsDtos
type ExerciseSetsDtoTag =
    | RepsSets = 1
    | RepsWeightSets = 2
    | DurationSets = 3
    | DurationWeightSets = 4
    | DurationDistanceSets = 5

[<CLIMutable>]
type ExerciseSetsDto =
    { Tag: ExerciseSetsDtoTag

      [<BsonIgnoreIfDefault>]
      RepsSets: ResizeArray<RepsSetDto>

      [<BsonIgnoreIfDefault>]
      RepsWeightSets: ResizeArray<RepsWeightSetDto>

      [<BsonIgnoreIfDefault>]
      DurationSets: ResizeArray<DurationSetDto>

      [<BsonIgnoreIfDefault>]
      DurationWeightSets: ResizeArray<DurationWeightSetDto>

      [<BsonIgnoreIfDefault>]
      DurationDistanceSets: ResizeArray<DurationDistanceSetDto> }

[<CLIMutable>]
type ExerciseCategoryDto =
    { Id: string
      Name: string
      OwnerId: string }

[<CLIMutable>]
type ExerciseTemplateDto =
    { Id: string
      CategoryId: string
      Name: string
      Notes: string
      RestTime: TimeSpan
      Sets: ExerciseSetsDto
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type WorkoutTemplateDto =
    { Id: string
      Name: string
      Goal: string
      Notes: string
      Schedule: ResizeArray<DayOfWeek>
      Exercises: ResizeArray<ExerciseTemplateDto>
      CreatedOn: DateTime
      LastModifiedOn: DateTime
      OwnerId: string }

[<CLIMutable>]
type ExerciseDto =
    { TemplateId: string
      Sets: ExerciseSetsDto
      StartedOn: DateTime
      CompletedOn: DateTime }

[<CLIMutable>]
type WorkoutDto =
    { Id: string
      TemplateId: string
      Exercises: ResizeArray<ExerciseDto>
      StartedOn: DateTime
      CompletedOn: DateTime
      OwnerId: string }

type SexDto =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type SportsmanDto =
    { Id: string
      Email: string
      FirstName: string
      LastName: string
      DateOfBirth: Nullable<DateTime>
      Sex: Nullable<SexDto> }
