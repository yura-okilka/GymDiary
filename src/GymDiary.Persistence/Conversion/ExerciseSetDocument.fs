namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseSetDocument =

    let fromRepsSet (domain: RepsSet) : ExerciseSetDocument =
        {
            OrderNum = domain.OrderNum |> PositiveInt.value
            Reps = domain.Reps |> PositiveInt.value |> Some
            EquipmentWeight = None
            Duration = None
            Distance = None
        }

    let fromRepsWeightSet (domain: RepsWeightSet) : ExerciseSetDocument =
        {
            OrderNum = domain.OrderNum |> PositiveInt.value
            Reps = domain.Reps |> PositiveInt.value |> Some
            EquipmentWeight = domain.EquipmentWeight |> float |> Some
            Duration = None
            Distance = None
        }

    let fromDurationSet (domain: DurationSet) : ExerciseSetDocument =
        {
            OrderNum = domain.OrderNum |> PositiveInt.value
            Reps = None
            EquipmentWeight = None
            Duration = domain.Duration |> Some
            Distance = None
        }

    let fromDurationWeightSet (domain: DurationWeightSet) : ExerciseSetDocument =
        {
            OrderNum = domain.OrderNum |> PositiveInt.value
            Reps = None
            EquipmentWeight = domain.EquipmentWeight |> float |> Some
            Duration = domain.Duration |> Some
            Distance = None
        }

    let fromDurationDistanceSet (domain: DurationDistanceSet) : ExerciseSetDocument =
        {
            OrderNum = domain.OrderNum |> PositiveInt.value
            Reps = None
            EquipmentWeight = None
            Duration = domain.Duration |> Some
            Distance = domain.Distance |> decimal |> Some
        }

    let fromExerciseSets (sets: ExerciseSets) =
        match sets with
        | RepsSets s -> (ExerciseSetType.RepsSet, s |> List.map fromRepsSet)
        | RepsWeightSets s -> (ExerciseSetType.RepsWeightSet, s |> List.map fromRepsWeightSet)
        | DurationSets s -> (ExerciseSetType.DurationSet, s |> List.map fromDurationSet)
        | DurationWeightSets s -> (ExerciseSetType.DurationWeightSet, s |> List.map fromDurationWeightSet)
        | DurationDistanceSets s -> (ExerciseSetType.DurationDistanceSet, s |> List.map fromDurationDistanceSet)

    let private requireSome field value =
        value |> Result.requireSome (ValidationError.valueNull field)

    let private toPositiveInt field value =
        value |> requireSome field |> Result.bind (PositiveInt.create field)

    let private toFloatKg field value = value |> requireSome field |> Result.map floatKg

    let toRepsSet (document: ExerciseSetDocument) : Result<RepsSet, ValidationError> =
        result {
            let! orderNum = document.OrderNum |> PositiveInt.create (nameof document.OrderNum)
            let! reps = document.Reps |> toPositiveInt (nameof document.Reps)

            return RepsSet.create orderNum reps
        }

    let toRepsWeightSet (document: ExerciseSetDocument) : Result<RepsWeightSet, ValidationError> =
        result {
            let! orderNum = document.OrderNum |> PositiveInt.create (nameof document.OrderNum)
            let! reps = document.Reps |> toPositiveInt (nameof document.Reps)
            let! weight = document.EquipmentWeight |> toFloatKg (nameof document.EquipmentWeight)

            return RepsWeightSet.create orderNum reps weight
        }

    let toDurationSet (document: ExerciseSetDocument) : Result<DurationSet, ValidationError> =
        result {
            let! orderNum = document.OrderNum |> PositiveInt.create (nameof document.OrderNum)
            let! duration = document.Duration |> requireSome (nameof document.Duration)

            return DurationSet.create orderNum duration
        }

    let toDurationWeightSet (document: ExerciseSetDocument) : Result<DurationWeightSet, ValidationError> =
        result {
            let! orderNum = document.OrderNum |> PositiveInt.create (nameof document.OrderNum)
            let! duration = document.Duration |> requireSome (nameof document.Duration)
            let! weight = document.EquipmentWeight |> toFloatKg (nameof document.EquipmentWeight)

            return DurationWeightSet.create orderNum duration weight
        }

    let toDurationDistanceSet (document: ExerciseSetDocument) : Result<DurationDistanceSet, ValidationError> =
        result {
            let! orderNum = document.OrderNum |> PositiveInt.create (nameof document.OrderNum)
            let! duration = document.Duration |> requireSome (nameof document.Duration)
            let! distance = document.Distance |> requireSome (nameof document.Duration) |> Result.map decimalM

            return DurationDistanceSet.create orderNum duration distance
        }

    let toExerciseSets (setType: ExerciseSetType) (documents: ExerciseSetDocument list) : Result<ExerciseSets, ValidationError> =
        match setType with
        | ExerciseSetType.RepsSet -> documents |> List.traverseResultM toRepsSet |> Result.map RepsSets

        | ExerciseSetType.RepsWeightSet -> documents |> List.traverseResultM toRepsWeightSet |> Result.map RepsWeightSets

        | ExerciseSetType.DurationSet -> documents |> List.traverseResultM toDurationSet |> Result.map DurationSets

        | ExerciseSetType.DurationWeightSet ->
            documents |> List.traverseResultM toDurationWeightSet |> Result.map DurationWeightSets

        | ExerciseSetType.DurationDistanceSet ->
            documents |> List.traverseResultM toDurationDistanceSet |> Result.map DurationDistanceSets

        | _ -> ValidationError.invalidValue (nameof setType) (string (setType)) |> Error
