namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling.Operator.Result

module ExerciseCategoryDto =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDto =
        { Id = domain.Id |> ExerciseCategoryId.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: ExerciseCategoryDto) : Result<ExerciseCategory, ValidationError> =
        let id = dto.Id |> ExerciseCategoryId.create "Id"
        let name = dto.Name |> String50.create "Name"
        let ownerId = dto.OwnerId |> SportsmanId.create "OwnerId"

        ExerciseCategory.create <!> id <*> name <*> ownerId
