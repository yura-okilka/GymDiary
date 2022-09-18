namespace GymDiary.Core.Workflows

open System

open Common.Extensions

open GymDiary.Core.Domain

open FsToolkit.ErrorHandling

module CommonDtos =

    type RepsSetDto = { OrderNum: int; Reps: int }

    type RepsWeightSetDto =
        {
            OrderNum: int
            Reps: int
            EquipmentWeight: float
        }

    type DurationSetDto = { OrderNum: int; Duration: TimeSpan }

    type DurationWeightSetDto =
        {
            OrderNum: int
            Duration: TimeSpan
            EquipmentWeight: float
        }

    type DurationDistanceSetDto =
        {
            OrderNum: int
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
            let toRepsSet (dto: RepsSetDto) =
                validation {
                    let! orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
                    and! reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)

                    return RepsSet.create orderNum reps
                }

            let toRepsWeightSet (dto: RepsWeightSetDto) =
                validation {
                    let! orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
                    and! reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)
                    let weight = dto.EquipmentWeight |> floatKg

                    return RepsWeightSet.create orderNum reps weight
                }

            let toDurationSet (dto: DurationSetDto) =
                validation {
                    let! orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)

                    return DurationSet.create orderNum dto.Duration
                }

            let toDurationWeightSet (dto: DurationWeightSetDto) =
                validation {
                    let! orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
                    let weight = dto.EquipmentWeight |> floatKg

                    return DurationWeightSet.create orderNum dto.Duration weight
                }

            let toDurationDistanceSet (dto: DurationDistanceSetDto) =
                validation {
                    let! orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
                    let distance = dto.Distance |> floatM

                    return DurationDistanceSet.create orderNum dto.Duration distance
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
