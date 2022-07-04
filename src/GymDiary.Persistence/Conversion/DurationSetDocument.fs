namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module DurationSetDocument =

    let fromDomain (domain: DurationSet) : DurationSetDocument =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Duration = domain.Duration }

    let toDomain (dto: DurationSetDocument) : Result<DurationSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let duration = dto.Duration |> Ok

        DurationSet.create <!> orderNum <*> duration