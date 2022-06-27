namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module RepsSetDocument =

    let fromDomain (domain: RepsSet) : RepsSetDocument =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Reps = domain.Reps |> PositiveInt.value }

    let toDomain (dto: RepsSetDocument) : Result<RepsSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)

        RepsSet.create <!> orderNum <*> reps
