namespace GymDiary.Domain.ExerciseDefinitions

open System
open FSharp.UMX
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

[<Measure>]
type exerciseDefinitionId

type ExerciseDefinitionId = Guid<exerciseDefinitionId>

type ExerciseDefinition =
    { Id: ExerciseDefinitionId
      CategoryId: ExerciseCategoryId
      Name: String50
      Notes: String1k option
      RestTime: TimeSpan
      Sets: ExerciseSetGroup
      OwnerId: UserId
      CreatedOnUtc: DateTime
      UpdatedOnUtc: DateTime }

type ExerciseDefinitionError =
    | MustHaveAtLeastOneSet

module ExerciseDefinition =
    // The set kind is encoded in the ExerciseSetGroup type, so "all sets are the same kind" holds by
    // construction; only the non-empty invariant still needs checking.
    let private validateSets sets =

        let isEmpty =
            match sets with
            | RepetitionSets s -> List.isEmpty s
            | WeightedRepetitionSets s -> List.isEmpty s
            | DurationSets s -> List.isEmpty s
            | WeightedDurationSets s -> List.isEmpty s
            | TimedDistanceSets s -> List.isEmpty s

        if isEmpty then
            Error MustHaveAtLeastOneSet
        else
            Ok()

    let create id categoryId name notes restTime sets ownerId utcNow : Result<ExerciseDefinition, ExerciseDefinitionError> =
        match validateSets sets with
        | Error error -> Error error
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

    let update categoryId name notes restTime sets definition utcNow : Result<ExerciseDefinition, ExerciseDefinitionError> =
        match validateSets sets with
        | Error error -> Error error
        | Ok() ->
            Ok
                { definition with
                    CategoryId = categoryId
                    Name = name
                    Notes = notes
                    RestTime = restTime
                    Sets = sets
                    UpdatedOnUtc = utcNow }
