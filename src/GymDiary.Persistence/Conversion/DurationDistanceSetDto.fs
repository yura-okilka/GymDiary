namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

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
