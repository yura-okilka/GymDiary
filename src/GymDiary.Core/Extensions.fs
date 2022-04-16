namespace GymDiary.Core

open System

module Extensions =

    module Option =

        let ofRecord (value: 'T) =
            match box value with
            | null -> None
            | _ -> Some value

    module Result =

        let getOk (result) =
            match result with
            | Ok v -> v
            | Error _ -> failwith "Result is error"

        let getError (result) =
            match result with
            | Ok _ -> failwith "Result is not error"
            | Error er -> er

    module DateOnly =

        let toDateTime (date: DateOnly) = date.ToDateTime(TimeOnly.MinValue)