namespace GymDiary.Domain.ExerciseDefinitions

open System
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseDefinition =
    { Id: Id<ExerciseDefinition>
      CategoryId: Id<ExerciseCategory>
      Name: String50
      Notes: String1k option
      RestTime: TimeSpan
      Sets: ExerciseSets
      OwnerId: Id<User>
      CreatedOnUtc: DateTime
      UpdatedOnUtc: DateTime }

type ExerciseDefinitionId = Id<ExerciseDefinition>

module ExerciseDefinition =
    // The set kind is encoded in the ExerciseSets type, so "all sets are the same kind" holds by
    // construction; only the non-empty invariant still needs checking.
    let private validateSets sets =

        let isEmpty =
            match sets with
            | RepetitionSets s -> List.isEmpty s
            | WeightedRepetitionSets s -> List.isEmpty s
            | DurationSets s -> List.isEmpty s
            | WeightedDurationSets s -> List.isEmpty s
            | DistanceDurationSets s -> List.isEmpty s

        if isEmpty then
            Error "Exercise must have at least one set"
        else
            Ok()

    let create id categoryId name notes restTime sets ownerId utcNow : Result<ExerciseDefinition, string> =
        match validateSets sets with
        | Error message -> Error message
        | Ok() ->
            Ok
                { Id = id
                  CategoryId = categoryId
                  Name = name
                  Notes = notes
                  RestTime = restTime
                  Sets = sets
                  OwnerId = ownerId
                  CreatedOnUtc = utcNow
                  UpdatedOnUtc = utcNow }

    let update categoryId name notes restTime sets definition utcNow : Result<ExerciseDefinition, string> =
        match validateSets sets with
        | Error message -> Error message
        | Ok() ->
            Ok
                { definition with
                    CategoryId = categoryId
                    Name = name
                    Notes = notes
                    RestTime = restTime
                    Sets = sets
                    UpdatedOnUtc = utcNow }
