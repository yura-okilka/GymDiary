namespace GymDiary.Core.Workflows

open System

open Common.Extensions

open GymDiary.Core.Domain

open FsToolkit.ErrorHandling

module CommonDtos =

    type RepsSetDto = { SequenceNumber: int; Reps: int }

    type RepsWeightSetDto = {
        SequenceNumber: int
        Reps: int
        EquipmentWeight: float
    }

    type DurationSetDto = { SequenceNumber: int; Duration: TimeSpan }

    type DurationWeightSetDto = {
        SequenceNumber: int
        Duration: TimeSpan
        EquipmentWeight: float
    }

    type DurationDistanceSetDto = {
        SequenceNumber: int
        Duration: TimeSpan
        Distance: float
    }

    type ExerciseSetsDto =
        | RepsSets of items: RepsSetDto list
        | RepsWeightSets of items: RepsWeightSetDto list
        | DurationSets of items: DurationSetDto list
        | DurationWeightSets of items: DurationWeightSetDto list
        | DurationDistanceSets of items: DurationDistanceSetDto list

        static member toDomain(setsDto: ExerciseSetsDto) : Result<ExerciseSets, ValidationError list> =
            let toRepsSet (dto: RepsSetDto) = validation {
                let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)
                and! reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)

                return RepsSet.create sequenceNumber reps
            }

            let toRepsWeightSet (dto: RepsWeightSetDto) = validation {
                let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)
                and! reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)
                let weight = dto.EquipmentWeight |> floatKg

                return RepsWeightSet.create sequenceNumber reps weight
            }

            let toDurationSet (dto: DurationSetDto) = validation {
                let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)

                return DurationSet.create sequenceNumber dto.Duration
            }

            let toDurationWeightSet (dto: DurationWeightSetDto) = validation {
                let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)
                let weight = dto.EquipmentWeight |> floatKg

                return DurationWeightSet.create sequenceNumber dto.Duration weight
            }

            let toDurationDistanceSet (dto: DurationDistanceSetDto) = validation {
                let! sequenceNumber = dto.SequenceNumber |> PositiveInt.create (nameof dto.SequenceNumber)
                let distance = dto.Distance |> floatM

                return DurationDistanceSet.create sequenceNumber dto.Duration distance
            }

            match setsDto with
            | RepsSets items -> items |> List.traverseValidationA toRepsSet |> Result.map ExerciseSets.RepsSets
            | RepsWeightSets items -> items |> List.traverseValidationA toRepsWeightSet |> Result.map ExerciseSets.RepsWeightSets
            | DurationSets items -> items |> List.traverseValidationA toDurationSet |> Result.map ExerciseSets.DurationSets
            | DurationWeightSets items ->
                items |> List.traverseValidationA toDurationWeightSet |> Result.map ExerciseSets.DurationWeightSets
            | DurationDistanceSets items ->
                items
                |> List.traverseValidationA toDurationDistanceSet
                |> Result.map ExerciseSets.DurationDistanceSets
