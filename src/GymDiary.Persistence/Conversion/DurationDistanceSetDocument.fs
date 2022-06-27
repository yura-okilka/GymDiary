namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module DurationDistanceSetDocument =

    let fromDomain (domain: DurationDistanceSet) : DurationDistanceSetDocument =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Duration = domain.Duration
          Distance = domain.Distance |> decimal }

    let toDomain (dto: DurationDistanceSetDocument) : Result<DurationDistanceSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let duration = dto.Duration |> Ok
        let distance = dto.Distance |> decimalM |> Ok

        DurationDistanceSet.create <!> orderNum <*> duration <*> distance
