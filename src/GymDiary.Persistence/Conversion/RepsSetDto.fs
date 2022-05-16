namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module RepsSetDto =

    let fromDomain (domain: RepsSet) : RepsSetDto =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Reps = domain.Reps |> PositiveInt.value }

    let toDomain (dto: RepsSetDto) : Result<RepsSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)

        RepsSet.create <!> orderNum <*> reps
