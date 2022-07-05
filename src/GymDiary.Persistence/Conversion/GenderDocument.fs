namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

module GenderDocument =

    let fromDomain (domain: Gender) : GenderDocument =
        match domain with
        | Male -> GenderDocument.Male
        | Female -> GenderDocument.Female
        | Other -> GenderDocument.Other

    let toDomain (field: string) (dto: GenderDocument) : Result<Gender, ValidationError> =
        match dto with
        | GenderDocument.Male -> Male |> Ok
        | GenderDocument.Female -> Female |> Ok
        | GenderDocument.Other -> Other |> Ok
        | _ -> ValidationError(field, InvalidValue(dto.ToString())) |> Error
