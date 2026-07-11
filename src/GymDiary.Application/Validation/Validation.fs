namespace GymDiary.Application.Validation

/// A field paired with its validation error messages.
type ValidationError =
    | ValidationError of field: string * errors: string list

module Validation =
    let checkField (fieldName: string) (validator: 'I -> Result<'V, string>) (input: 'I) =
        input |> validator |> Result.mapError (fun error -> ValidationError(fieldName, [ error ]))
