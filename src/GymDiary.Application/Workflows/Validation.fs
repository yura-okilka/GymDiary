namespace GymDiary.Application.Workflows.Validation

open System.Collections.Generic

/// A mapping of fields to their validation errors.
type ValidationErrors =
    private
    | ValidationErrors of Map<string, string list>

    static member ofField field error = ValidationErrors(Map [ field, [ error ] ])

    /// Combines many sets of errors into one, concatenating messages that share a
    /// field. Accumulates through a mutable dictionary to avoid rebuilding the
    /// immutable map on every entry, then freezes the result back into a map.
    /// Folds the `ValidationErrors list` that FsToolkit's `validation` CE accumulates
    /// into a single grouped set.
    static member collect(errorSets: ValidationErrors seq) =
        let acc = Dictionary<string, string list>()

        for ValidationErrors map in errorSets do
            for KeyValue(field, errors) in map do
                match acc.TryGetValue field with
                | true, existing -> acc[field] <- existing @ errors
                | false, _ -> acc[field] <- errors

        acc |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq |> ValidationErrors

    /// Combines two sets of errors, concatenating messages that share a field.
    static member merge (x: ValidationErrors) (y: ValidationErrors) = ValidationErrors.collect [ x; y ]

    /// Projects the errors into the field → messages dictionary shape used by
    /// RFC 7807 validation problem details.
    member this.ToDictionary() : IDictionary<string, string[]> =
        let (ValidationErrors map) = this
        let dict = Dictionary<string, string[]>(map.Count)
        map |> Map.iter (fun field errors -> dict[field] <- List.toArray errors)
        dict

module Validation =
    let checkField (fieldName: string) (validator: 'I -> Result<'V, string>) (input: 'I) =
        input |> validator |> Result.mapError (ValidationErrors.ofField fieldName)
