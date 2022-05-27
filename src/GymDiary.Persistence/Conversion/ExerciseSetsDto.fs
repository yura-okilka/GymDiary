namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FSharpx.Collections

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module ExerciseSetsDto =

    let fromDomain (domain: ExerciseSets) : ExerciseSetsDto =
        let emptyDto =
            { Tag = defaultof<ExerciseSetsDtoTag>
              RepsSets = null
              RepsWeightSets = null
              DurationSets = null
              DurationDistanceSets = null
              DurationWeightSets = null }

        match domain with
        | RepsSets sets ->
            let repsSets = sets |> Seq.ofList |> Seq.map RepsSetDto.fromDomain |> ResizeArray<RepsSetDto>

            { emptyDto with
                Tag = ExerciseSetsDtoTag.RepsSets
                RepsSets = repsSets }

        | RepsWeightSets sets ->
            let repsWeightSets =
                sets |> Seq.ofList |> Seq.map RepsWeightSetDto.fromDomain |> ResizeArray<RepsWeightSetDto>

            { emptyDto with
                Tag = ExerciseSetsDtoTag.RepsWeightSets
                RepsWeightSets = repsWeightSets }

        | DurationSets sets ->
            let durationSets =
                sets |> Seq.ofList |> Seq.map DurationSetDto.fromDomain |> ResizeArray<DurationSetDto>

            { emptyDto with
                Tag = ExerciseSetsDtoTag.DurationSets
                DurationSets = durationSets }

        | DurationDistanceSets sets ->
            let durationDistanceSets =
                sets
                |> Seq.ofList
                |> Seq.map DurationDistanceSetDto.fromDomain
                |> ResizeArray<DurationDistanceSetDto>

            { emptyDto with
                Tag = ExerciseSetsDtoTag.DurationDistanceSets
                DurationDistanceSets = durationDistanceSets }

        | DurationWeightSets sets ->
            let durationWeightSets =
                sets |> Seq.ofList |> Seq.map DurationWeightSetDto.fromDomain |> ResizeArray<DurationWeightSetDto>

            { emptyDto with
                Tag = ExerciseSetsDtoTag.DurationWeightSets
                DurationWeightSets = durationWeightSets }

    let toDomain (dto: ExerciseSetsDto) : Result<ExerciseSets, ValidationError> =
        match dto.Tag with
        | ExerciseSetsDtoTag.RepsSets ->
            if dto.RepsSets = null then
                ValidationError(nameof dto.RepsSets, ValueNull) |> Error
            else
                dto.RepsSets
                |> ResizeArray.toList
                |> List.traverseResultM RepsSetDto.toDomain
                |> Result.map RepsSets

        | ExerciseSetsDtoTag.RepsWeightSets ->
            if dto.RepsWeightSets = null then
                ValidationError(nameof dto.RepsWeightSets, ValueNull) |> Error
            else
                dto.RepsWeightSets
                |> ResizeArray.toList
                |> List.traverseResultM RepsWeightSetDto.toDomain
                |> Result.map RepsWeightSets

        | ExerciseSetsDtoTag.DurationSets ->
            if dto.DurationSets = null then
                ValidationError(nameof dto.DurationSets, ValueNull) |> Error
            else
                dto.DurationSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationSetDto.toDomain
                |> Result.map DurationSets

        | ExerciseSetsDtoTag.DurationWeightSets ->
            if dto.DurationWeightSets = null then
                ValidationError(nameof dto.DurationWeightSets, ValueNull) |> Error
            else
                dto.DurationWeightSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationWeightSetDto.toDomain
                |> Result.map DurationWeightSets

        | ExerciseSetsDtoTag.DurationDistanceSets ->
            if dto.DurationDistanceSets = null then
                ValidationError(nameof dto.DurationDistanceSets, ValueNull) |> Error
            else
                dto.DurationDistanceSets
                |> ResizeArray.toList
                |> List.traverseResultM DurationDistanceSetDto.toDomain
                |> Result.map DurationDistanceSets

        | _ -> ValidationError(nameof dto.Tag, InvalidValue(dto.Tag.ToString())) |> Error
