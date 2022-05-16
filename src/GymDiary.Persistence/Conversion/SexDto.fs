namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

module SexDto =

    let fromDomain (domain: Sex) : SexDto =
        match domain with
        | Male -> SexDto.Male
        | Female -> SexDto.Female
        | Other -> SexDto.Other

    let toDomain (field: string) (dto: SexDto) : Result<Sex, ValidationError> =
        match dto with
        | SexDto.Male -> Male |> Ok
        | SexDto.Female -> Female |> Ok
        | SexDto.Other -> Other |> Ok
        | _ -> ValidationError(field, InvalidValue(dto.ToString())) |> Error
