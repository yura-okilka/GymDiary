namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling.Operator.Result

module DurationDistanceSetDto =

    let fromDomain (domain: DurationDistanceSet) : DurationDistanceSetDto =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Duration = domain.Duration
          Distance = domain.Distance |> decimal }

    let toDomain (dto: DurationDistanceSetDto) : Result<DurationDistanceSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let duration = dto.Duration |> Ok
        let distance = dto.Distance |> decimalM |> Ok

        DurationDistanceSet.create <!> orderNum <*> duration <*> distance
