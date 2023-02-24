namespace GymDiary.Core.Domain

module RepsSet =

    let create orderNum reps : RepsSet = { OrderNum = orderNum; Reps = reps }

module RepsWeightSet =

    let create orderNum reps equipmentWeight : RepsWeightSet = {
        OrderNum = orderNum
        Reps = reps
        EquipmentWeight = equipmentWeight
    }

module DurationSet =

    let create orderNum duration : DurationSet = {
        OrderNum = orderNum
        Duration = duration
    }

module DurationWeightSet =

    let create orderNum duration equipmentWeight : DurationWeightSet = {
        OrderNum = orderNum
        Duration = duration
        EquipmentWeight = equipmentWeight
    }

module DurationDistanceSet =

    let create orderNum duration distance : DurationDistanceSet = {
        OrderNum = orderNum
        Duration = duration
        Distance = distance
    }
