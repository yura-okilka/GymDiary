namespace GymDiary.Domain.ExerciseDefinitions

open System
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users

type ExerciseDefinition = {
    Id: Id<ExerciseDefinition>
    CategoryId: Id<ExerciseCategory>
    Name: String50
    Notes: String1k option
    RestTime: TimeSpan
    Sets: ExerciseSet list
    OwnerId: Id<User>
}

type ExerciseDefinitionId = Id<ExerciseDefinition>

module ExerciseDefinition =
    let private validateSets sets =
        if List.isEmpty sets then
            Error "Exercise must have at least one set"
        elif sets |> Seq.map _.Kind |> Seq.distinct |> Seq.length > 1 then
            Error "All sets must be of the same kind"
        else
            Ok()

    let create id categoryId name notes restTime sets ownerId : Result<ExerciseDefinition, string> =
        match validateSets sets with
        | Error message -> Error message
        | Ok() ->
            Ok {
                Id = id
                CategoryId = categoryId
                Name = name
                Notes = notes
                RestTime = restTime
                Sets = sets
                OwnerId = ownerId
            }

    let update categoryId name notes restTime sets definition : Result<ExerciseDefinition, string> =
        match validateSets sets with
        | Error message -> Error message
        | Ok() ->
            Ok {
                definition with
                    CategoryId = categoryId
                    Name = name
                    Notes = notes
                    RestTime = restTime
                    Sets = sets
            }
