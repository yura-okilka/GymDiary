namespace GymDiary.Infrastructure.Persistence.Mapping

open Common.Extensions
open FsToolkit.ErrorHandling
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation

type ExerciseSetDtoMapper() =
    interface IDocumentMapper<ExerciseSet, ExerciseSetDto> with

        member _.MapFromDomain(domain: ExerciseSet) : ExerciseSetDto =
            let setKindToDto =
                function
                | Repetitions -> ExerciseSetKindDto.Repetitions
                | RepetitionsWithWeight -> ExerciseSetKindDto.RepetitionsWithWeight
                | Duration -> ExerciseSetKindDto.Duration
                | DurationWithWeight -> ExerciseSetKindDto.DurationWithWeight
                | DurationWithDistance -> ExerciseSetKindDto.DurationWithDistance

            {
                Kind = setKindToDto domain.Kind
                Repetitions = domain.Repetitions
                Weight = float domain.Weight
                Distance = float domain.Distance
                Duration = domain.Duration
            }

        member _.MapToDomain(document: ExerciseSetDto) : Result<ExerciseSet, ValidationError> = result {
            let dtoToSetKind field value =
                match value with
                | ExerciseSetKindDto.Repetitions -> Repetitions |> Ok
                | ExerciseSetKindDto.RepetitionsWithWeight -> RepetitionsWithWeight |> Ok
                | ExerciseSetKindDto.Duration -> Duration |> Ok
                | ExerciseSetKindDto.DurationWithWeight -> DurationWithWeight |> Ok
                | ExerciseSetKindDto.DurationWithDistance -> DurationWithDistance |> Ok
                | _ -> ValidationError(field, $"{value} is not a valid {nameof ExerciseSetKindDto}") |> Error // TODO: add helper function

            let! kind = document.Kind |> dtoToSetKind (nameof document.Kind)

            return {
                Kind = kind
                Repetitions = document.Repetitions
                Weight = floatKg document.Weight
                Distance = floatM document.Distance
                Duration = document.Duration
            }
        }
