module GymDiary.Core.Domain.DomainTypes

open System
open CommonTypes

type ExerciseCategoryId = private ExerciseCategoryId of Guid
type ExerciseTemplateId = private ExerciseTemplateId of Guid
type WorkoutTemplateId = private WorkoutTemplateId of Guid
type WorkoutId = private WorkoutId of Guid
type SportsmanId = private SportsmanId of Guid

type RepsSet = private {
    OrderNum: PositiveInt
    Reps: PositiveInt
}

type RepsWeightSet = private {
    OrderNum: PositiveInt
    Reps: PositiveInt
    EquipmentWeight: Kilogram
}

type DurationSet = private {
    OrderNum: PositiveInt
    Duration: TimeSpan
}

type DurationWeightSet = private {
    OrderNum: PositiveInt
    Duration: TimeSpan
    EquipmentWeight: Kilogram
}

type DurationDistanceSet = private {
    OrderNum: PositiveInt
    Duration: TimeSpan
    Distance: Kilometer
}

type ExerciseSets = 
    private
    | RepsSets of RepsSet list
    | RepsWeightSets of RepsWeightSet list
    | DurationSets of DurationSet list
    | DurationWeightSets of DurationWeightSet list
    | DurationDistanceSets of DurationDistanceSet list
    
type ExerciseCategory = private {
    Id: ExerciseCategoryId
    Name: String50
}

type ExerciseTemplate = private {
    Id: ExerciseTemplateId
    CategoryId: ExerciseCategoryId
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSets
    CreatedOn: DateTimeOffset
    LastModifiedOn: DateTimeOffset
}

type WorkoutTemplate = private {
    Id: WorkoutTemplateId
    Name: String50
    Goal: String200 option
    Notes: String1k option
    Schedule: DayOfWeek Set
    Exercises: ExerciseTemplate list
    CreatedOn: DateTimeOffset
    LastModifiedOn: DateTimeOffset
}

type Exercise = private {
    TemplateId: ExerciseTemplateId
    Sets: ExerciseSets
    StartedOn: DateTimeOffset
    CompletedOn: DateTimeOffset
}

type Workout = private {
    Id: WorkoutId
    TemplateId: WorkoutTemplateId
    Exercises: Exercise list
    StartedOn: DateTimeOffset
    CompletedOn: DateTimeOffset
}

type Sportsman = private {
    Id: SportsmanId
    Email: EmailAddress
    FirstName: String50
    LastName: String50
    DateOfBirth: DateOnly option
    Sex: Sex option
    Workouts: WorkoutId list
    WorkoutTemplates: WorkoutTemplateId list
    ExerciseTemplates: ExerciseTemplateId list
    ExerciseCategories: ExerciseCategoryId list
}