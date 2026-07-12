namespace GymDiary.Persistence.MongoDB.Mapping

open Common.Extensions
open FsToolkit.ErrorHandling
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Validation

// Both the domain group and its DTO are discriminated unions with the same case names, so the mapping
// is a straight structural translation, case for case. The "same kind" / "valid kind" checks the old
// flat-list mapper needed are gone: a mixed or unknown kind is unrepresentable in either union.
type ExerciseSetGroupMapper() =

    let toPositive value =
        value |> Validation.checkField "Repetitions" PositiveInt.create

    interface IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto> with
        member _.MapFromDomain(domain: ExerciseSetGroup) : ExerciseSetGroupDto =
            match domain with
            | ExerciseSetGroup.RepetitionSets reps -> ExerciseSetGroupDto.RepetitionSets(reps |> List.map _.Value)
            | ExerciseSetGroup.WeightedRepetitionSets items ->
                ExerciseSetGroupDto.WeightedRepetitionSets(
                    items |> List.map (fun (r, w) -> { Repetitions = r.Value; Weight = float w })
                )
            | ExerciseSetGroup.DurationSets durations -> ExerciseSetGroupDto.DurationSets durations
            | ExerciseSetGroup.WeightedDurationSets items ->
                ExerciseSetGroupDto.WeightedDurationSets(
                    items |> List.map (fun (d, w) -> { Duration = d; Weight = float w })
                )
            | ExerciseSetGroup.TimedDistanceSets items ->
                ExerciseSetGroupDto.TimedDistanceSets(
                    items |> List.map (fun (dist, d) -> { Distance = float dist; Duration = d })
                )

        member _.MapToDomain(document: ExerciseSetGroupDto) : Result<ExerciseSetGroup, ValidationError> = result {
            match document with
            | ExerciseSetGroupDto.RepetitionSets reps ->
                let! reps = reps |> List.traverseResultM toPositive
                return ExerciseSetGroup.RepetitionSets reps
            | ExerciseSetGroupDto.WeightedRepetitionSets sets ->
                let! items =
                    sets
                    |> List.traverseResultM (fun s -> toPositive s.Repetitions |> Result.map (fun r -> (r, floatKg s.Weight)))

                return ExerciseSetGroup.WeightedRepetitionSets items
            | ExerciseSetGroupDto.DurationSets durations -> return ExerciseSetGroup.DurationSets durations
            | ExerciseSetGroupDto.WeightedDurationSets sets ->
                return ExerciseSetGroup.WeightedDurationSets(sets |> List.map (fun s -> (s.Duration, floatKg s.Weight)))
            | ExerciseSetGroupDto.TimedDistanceSets sets ->
                return ExerciseSetGroup.TimedDistanceSets(sets |> List.map (fun s -> (floatM s.Distance, s.Duration)))
        }
