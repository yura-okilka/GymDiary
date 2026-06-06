namespace GymDiary.Persistence.MongoDB.Mapping

open System

open Common.Extensions
open FsToolkit.ErrorHandling
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Workflows.Validation

type ExerciseSetDtoMapper() =

    let emptySetDto kind = {
        Kind = kind
        Repetitions = 0u
        Weight = 0.0
        Distance = 0.0
        Duration = TimeSpan.Zero
    }

    // The group DU is stored as a flat list of kind-tagged set documents (one per set), so the storage
    // schema is unchanged; the kind is recovered on the way back in and validated to be uniform.
    interface IDocumentMapper<ExerciseSets, ExerciseSetDto list> with
        member _.MapFromDomain(domain: ExerciseSets) : ExerciseSetDto list =
            match domain with
            | RepetitionSets reps ->
                reps |> List.map (fun r -> { emptySetDto ExerciseSetKindDto.Repetitions with Repetitions = uint r.Value })
            | WeightedRepetitionSets items ->
                items
                |> List.map (fun (r, w) -> { emptySetDto ExerciseSetKindDto.RepetitionsWithWeight with Repetitions = uint r.Value; Weight = float w })
            | DurationSets durations ->
                durations |> List.map (fun d -> { emptySetDto ExerciseSetKindDto.Duration with Duration = d })
            | WeightedDurationSets items ->
                items |> List.map (fun (d, w) -> { emptySetDto ExerciseSetKindDto.DurationWithWeight with Duration = d; Weight = float w })
            | DistanceDurationSets items ->
                items |> List.map (fun (d, dist) -> { emptySetDto ExerciseSetKindDto.DurationWithDistance with Duration = d; Distance = float dist })

        member _.MapToDomain(documents: ExerciseSetDto list) : Result<ExerciseSets, ValidationError> = result {
            let positiveReps (d: ExerciseSetDto) =
                int d.Repetitions |> Validation.checkField (nameof d.Repetitions) PositiveInt.create

            match documents with
            | [] -> return! ValidationError.ofField "Sets" "Exercise must have at least one set" |> Error
            | first :: _ ->
                if documents |> List.exists (fun d -> d.Kind <> first.Kind) then
                    return! ValidationError.ofField "Sets" "All sets must be of the same kind" |> Error
                else
                    match first.Kind with
                    | ExerciseSetKindDto.Repetitions ->
                        let! reps = documents |> List.traverseResultM positiveReps
                        return RepetitionSets reps
                    | ExerciseSetKindDto.RepetitionsWithWeight ->
                        let! items =
                            documents
                            |> List.traverseResultM (fun d -> positiveReps d |> Result.map (fun r -> (r, floatKg d.Weight)))

                        return WeightedRepetitionSets items
                    | ExerciseSetKindDto.Duration -> return DurationSets(documents |> List.map _.Duration)
                    | ExerciseSetKindDto.DurationWithWeight ->
                        return WeightedDurationSets(documents |> List.map (fun d -> (d.Duration, floatKg d.Weight)))
                    | ExerciseSetKindDto.DurationWithDistance ->
                        return DistanceDurationSets(documents |> List.map (fun d -> (d.Duration, floatM d.Distance)))
                    | other ->
                        return! ValidationError.ofField (nameof first.Kind) $"{other} is not a valid {nameof ExerciseSetKindDto}" |> Error
        }
