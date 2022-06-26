namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module ExerciseCategoryDto =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDto =
        { Id = domain.Id |> Id.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (dto: ExerciseCategoryDto) : Result<ExerciseCategory, ValidationError> =
        let id = dto.Id |> Id.create (nameof dto.Id)
        let name = dto.Name |> String50.create (nameof dto.Name)
        let ownerId = dto.OwnerId |> Id.create (nameof dto.OwnerId)

        ExerciseCategory.create <!> id <*> name <*> ownerId
