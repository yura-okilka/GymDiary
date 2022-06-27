namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module RepsWeightSetDocument =

    let fromDomain (domain: RepsWeightSet) : RepsWeightSetDocument =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Reps = domain.Reps |> PositiveInt.value
          EquipmentWeight = domain.EquipmentWeight |> EquipmentWeightKg.value }

    let toDomain (dto: RepsWeightSetDocument) : Result<RepsWeightSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let reps = dto.Reps |> PositiveInt.create (nameof dto.Reps)
        let equipmentWeight = dto.EquipmentWeight |> EquipmentWeightKg.create (nameof dto.EquipmentWeight)

        RepsWeightSet.create <!> orderNum <*> reps <*> equipmentWeight
