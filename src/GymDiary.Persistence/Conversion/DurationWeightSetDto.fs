namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module DurationWeightSetDto =

    let fromDomain (domain: DurationWeightSet) : DurationWeightSetDto =
        { OrderNum = domain.OrderNum |> PositiveInt.value
          Duration = domain.Duration
          EquipmentWeight = domain.EquipmentWeight |> EquipmentWeightKg.value }

    let toDomain (dto: DurationWeightSetDto) : Result<DurationWeightSet, ValidationError> =
        let orderNum = dto.OrderNum |> PositiveInt.create (nameof dto.OrderNum)
        let duration = dto.Duration |> Ok
        let equipmentWeight = dto.EquipmentWeight |> EquipmentWeightKg.create (nameof dto.EquipmentWeight)

        DurationWeightSet.create <!> orderNum <*> duration <*> equipmentWeight
