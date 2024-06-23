namespace GymDiary.Persistence.Conversion

open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Persistence
open FsToolkit.ErrorHandling

module ExerciseSetDocument =

    let fromRepsSet (domain: RepsSet) : ExerciseSetDocument = {
        SequenceNumber = domain.SequenceNumber |> PositiveInt.value
        Reps = domain.Reps |> PositiveInt.value |> Some
        EquipmentWeight = None
        Duration = None
        Distance = None
    }

    let fromRepsWeightSet (domain: RepsWeightSet) : ExerciseSetDocument = {
        SequenceNumber = domain.SequenceNumber |> PositiveInt.value
        Reps = domain.Reps |> PositiveInt.value |> Some
        EquipmentWeight = domain.EquipmentWeight |> float |> Some
        Duration = None
        Distance = None
    }

    let fromDurationSet (domain: DurationSet) : ExerciseSetDocument = {
        SequenceNumber = domain.SequenceNumber |> PositiveInt.value
        Reps = None
        EquipmentWeight = None
        Duration = domain.Duration |> Some
        Distance = None
    }

    let fromDurationWeightSet (domain: DurationWeightSet) : ExerciseSetDocument = {
        SequenceNumber = domain.SequenceNumber |> PositiveInt.value
        Reps = None
        EquipmentWeight = domain.EquipmentWeight |> float |> Some
        Duration = domain.Duration |> Some
        Distance = None
    }

    let fromDurationDistanceSet (domain: DurationDistanceSet) : ExerciseSetDocument = {
        SequenceNumber = domain.SequenceNumber |> PositiveInt.value
        Reps = None
        EquipmentWeight = None
        Duration = domain.Duration |> Some
        Distance = domain.Distance |> float |> Some
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

    let toRepsSet (document: ExerciseSetDocument) : Result<RepsSet, ValidationError> = result {
        let! sequenceNumber = document.SequenceNumber |> PositiveInt.create (nameof document.SequenceNumber)
        let! reps = document.Reps |> toPositiveInt (nameof document.Reps)

        return RepsSet.create sequenceNumber reps
    }

    let toRepsWeightSet (document: ExerciseSetDocument) : Result<RepsWeightSet, ValidationError> = result {
        let! sequenceNumber = document.SequenceNumber |> PositiveInt.create (nameof document.SequenceNumber)
        let! reps = document.Reps |> toPositiveInt (nameof document.Reps)
        let! weight = document.EquipmentWeight |> toFloatKg (nameof document.EquipmentWeight)

        return RepsWeightSet.create sequenceNumber reps weight
    }

    let toDurationSet (document: ExerciseSetDocument) : Result<DurationSet, ValidationError> = result {
        let! sequenceNumber = document.SequenceNumber |> PositiveInt.create (nameof document.SequenceNumber)
        let! duration = document.Duration |> requireSome (nameof document.Duration)

        return DurationSet.create sequenceNumber duration
    }

    let toDurationWeightSet (document: ExerciseSetDocument) : Result<DurationWeightSet, ValidationError> = result {
        let! sequenceNumber = document.SequenceNumber |> PositiveInt.create (nameof document.SequenceNumber)
        let! duration = document.Duration |> requireSome (nameof document.Duration)
        let! weight = document.EquipmentWeight |> toFloatKg (nameof document.EquipmentWeight)

        return DurationWeightSet.create sequenceNumber duration weight
    }

    let toDurationDistanceSet (document: ExerciseSetDocument) : Result<DurationDistanceSet, ValidationError> = result {
        let! sequenceNumber = document.SequenceNumber |> PositiveInt.create (nameof document.SequenceNumber)
        let! duration = document.Duration |> requireSome (nameof document.Duration)
        let! distance = document.Distance |> requireSome (nameof document.Duration) |> Result.map floatM

        return DurationDistanceSet.create sequenceNumber duration distance
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
