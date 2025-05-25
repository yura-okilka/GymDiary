namespace GymDiary.Application.Workflows.Validation

type ValidationError =
    | ValidationError of field: string * error: string

    static member ofField field error = ValidationError(field, error)

module Validation =
    let checkField (fieldName: string) (validator: 'I -> Result<'V, string>) (input: 'I) =
        input |> validator |> Result.mapError (ValidationError.ofField fieldName)
