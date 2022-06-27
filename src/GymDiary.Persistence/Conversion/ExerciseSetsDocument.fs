namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FSharpx.Collections

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module ExerciseSetsDocument =

    let fromDomain (domain: ExerciseSets) : ExerciseSetsDocument =
        let emptyDocument =
            { Tag = defaultof<ExerciseSetsDocumentTag>
              RepsSets = null
              RepsWeightSets = null
              DurationSets = null
              DurationDistanceSets = null
              DurationWeightSets = null }

        match domain with
        | RepsSets sets ->
            let repsSets = sets |> Seq.ofList |> Seq.map RepsSetDocument.fromDomain |> ResizeArray<RepsSetDocument>

            { emptyDocument with
                Tag = ExerciseSetsDocumentTag.RepsSets
                RepsSets = repsSets }

        | RepsWeightSets sets ->
            let repsWeightSets =
                sets |> Seq.ofList |> Seq.map RepsWeightSetDocument.fromDomain |> ResizeArray<RepsWeightSetDocument>

            { emptyDocument with
                Tag = ExerciseSetsDocumentTag.RepsWeightSets
                RepsWeightSets = repsWeightSets }

        | DurationSets sets ->
            let durationSets =
                sets |> Seq.ofList |> Seq.map DurationSetDocument.fromDomain |> ResizeArray<DurationSetDocument>

            { emptyDocument with
                Tag = ExerciseSetsDocumentTag.DurationSets
                DurationSets = durationSets }

        | DurationDistanceSets sets ->
            let durationDistanceSets =
                sets
                |> Seq.ofList
                |> Seq.map DurationDistanceSetDocument.fromDomain
                |> ResizeArray<DurationDistanceSetDocument>

            { emptyDocument with
                Tag = ExerciseSetsDocumentTag.DurationDistanceSets
                DurationDistanceSets = durationDistanceSets }

        | DurationWeightSets sets ->
            let durationWeightSets =
                sets |> Seq.ofList |> Seq.map DurationWeightSetDocument.fromDomain |> ResizeArray<DurationWeightSetDocument>

            { emptyDocument with
                Tag = ExerciseSetsDocumentTag.DurationWeightSets
                DurationWeightSets = durationWeightSets }

    let toDomain (dto: ExerciseSetsDocument) : Result<ExerciseSets, ValidationError> =
        match dto.Tag with
        | ExerciseSetsDocumentTag.RepsSets ->
            if dto.RepsSets = null then
                ValidationError(nameof dto.RepsSets, ValueNull) |> Error
            else
                dto.RepsSets
                |> ResizeArray.toList
                |> List.traverseResultM RepsSetDocument.toDomain
                |> Result.map RepsSets

        | ExerciseSetsDocumentTag.RepsWeightSets ->
            if dto.RepsWeightSets = null then
                ValidationError(nameof dto.RepsWeightSets, ValueNull) |> Error
            else
                dto.RepsWeightSets
                |> ResizeArray.toList
                |> List.traverseResultM RepsWeightSetDocument.toDomain
                |> Result.map RepsWeightSets

        | ExerciseSetsDocumentTag.DurationSets ->
            if dto.DurationSets = null then
                ValidationError(nameof dto.DurationSets, ValueNull) |> Error
            else
                dto.DurationSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationSetDocument.toDomain
                |> Result.map DurationSets

        | ExerciseSetsDocumentTag.DurationWeightSets ->
            if dto.DurationWeightSets = null then
                ValidationError(nameof dto.DurationWeightSets, ValueNull) |> Error
            else
                dto.DurationWeightSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationWeightSetDocument.toDomain
                |> Result.map DurationWeightSets

        | ExerciseSetsDocumentTag.DurationDistanceSets ->
            if dto.DurationDistanceSets = null then
                ValidationError(nameof dto.DurationDistanceSets, ValueNull) |> Error
            else
                dto.DurationDistanceSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationDistanceSetDocument.toDomain
                |> Result.map DurationDistanceSets

        | _ -> ValidationError(nameof dto.Tag, InvalidValue(dto.Tag.ToString())) |> Error
