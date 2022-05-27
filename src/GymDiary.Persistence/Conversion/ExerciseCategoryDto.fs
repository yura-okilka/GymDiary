namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Core.Domain.Logic
open GymDiary.Persistence

open FsToolkit.ErrorHandling.Operator.Result

module ExerciseCategoryDto =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDto =
        { Id = domain.Id |> ExerciseCategoryId.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: ExerciseCategoryDto) : Result<ExerciseCategory, ValidationError> =
        let id = dto.Id |> ExerciseCategoryId.create (nameof dto.Id)
        let name = dto.Name |> String50.create (nameof dto.Name)
        let ownerId = dto.OwnerId |> SportsmanId.create (nameof dto.OwnerId)

        ExerciseCategory.create <!> id <*> name <*> ownerId
