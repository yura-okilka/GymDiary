namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

module SexDocument =

    let fromDomain (domain: Sex) : SexDocument =
        match domain with
        | Male -> SexDocument.Male
        | Female -> SexDocument.Female
        | Other -> SexDocument.Other

    let toDomain (field: string) (dto: SexDocument) : Result<Sex, ValidationError> =
        match dto with
        | SexDocument.Male -> Male |> Ok
        | SexDocument.Female -> Female |> Ok
        | SexDocument.Other -> Other |> Ok
        | _ -> ValidationError(field, InvalidValue(dto.ToString())) |> Error
