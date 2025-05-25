namespace GymDiary.Persistence.Documents

open System
open Common.Extensions
open GymDiary.Core.Domain
open FsToolkit.ErrorHandling

type ExerciseSetTypeDto =
    | Repetitions = 1
    | RepetitionsWithWeight = 2
    | Duration = 3
    | DurationWithWeight = 4
    | DurationWithDistance = 5

[<CLIMutable>]
type ExerciseSetDataDto = {
    SequenceNumber: int
    Repetitions: uint
    Weight: float
    Distance: float
    Duration: TimeSpan
}

[<CLIMutable>]
type ExerciseSetsDto = {
    Type: ExerciseSetTypeDto
    Items: ExerciseSetDataDto list
} with

    static member fromDomain(domain: ExerciseSets) : ExerciseSetsDto =
        let typeToDto =
            function
            | Repetitions -> ExerciseSetTypeDto.Repetitions
            | RepetitionsWithWeight -> ExerciseSetTypeDto.RepetitionsWithWeight
            | Duration -> ExerciseSetTypeDto.Duration
            | DurationWithWeight -> ExerciseSetTypeDto.DurationWithWeight
            | DurationWithDistance -> ExerciseSetTypeDto.DurationWithDistance

        let dataToDto (i: ExerciseSetData) = {
            SequenceNumber = i.SequenceNumber |> PositiveInt.value
            Repetitions = i.Repetitions
            Weight = float i.Weight
            Distance = float i.Distance
            Duration = i.Duration
        }

        {
            Type = domain.Type |> typeToDto
            Items = domain.Items |> List.map dataToDto
        }

    static member toDomain(document: ExerciseSetsDto) : Result<ExerciseSets, ValidationError> = result {
        let dtoToType field value =
            match value with
            | ExerciseSetTypeDto.Repetitions -> Repetitions |> Ok
            | ExerciseSetTypeDto.RepetitionsWithWeight -> RepetitionsWithWeight |> Ok
            | ExerciseSetTypeDto.Duration -> Duration |> Ok
            | ExerciseSetTypeDto.DurationWithWeight -> DurationWithWeight |> Ok
            | ExerciseSetTypeDto.DurationWithDistance -> DurationWithDistance |> Ok
            | _ -> ValidationError.invalidValue field (nameof value) |> Error

        let dtoToData (dto: ExerciseSetDataDto) : Result<ExerciseSetData, ValidationError> = result {
            let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)

            return {
                SequenceNumber = sequenceNumber
                Repetitions = dto.Repetitions
                Weight = floatKg dto.Weight
                Distance = floatM dto.Distance
                Duration = dto.Duration
            }
        }

        let! t = document.Type |> dtoToType (nameof document.Type)
        let! items = document.Items |> List.traverseResultM dtoToData

        return { Type = t; Items = items }
    }
