namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.InternalExtensions
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling

module ExerciseTemplateDto =

    let fromDomain (domain: ExerciseTemplate) : ExerciseTemplateDto =
        { Id = domain.Id |> ExerciseTemplateId.value
          CategoryId = domain.CategoryId |> ExerciseCategoryId.value
          Name = domain.Name |> String50.value
          Notes = domain.Notes |> Option.map String1k.value |> Option.defaultValue aNull<string>
          RestTime = domain.RestTime
          Sets = domain.Sets |> ExerciseSetsDto.fromDomain
          CreatedOn = domain.CreatedOn
          LastModifiedOn = domain.LastModifiedOn
          OwnerId = domain.OwnerId |> SportsmanId.value }

    let toDomain (dto: ExerciseTemplateDto) : Result<ExerciseTemplate, ValidationError> =
        result {
            let! id = dto.Id |> ExerciseTemplateId.create (nameof dto.Id)
            let! categoryId = dto.CategoryId |> ExerciseCategoryId.create (nameof dto.CategoryId)
            let! name = dto.Name |> String50.create (nameof dto.Name)
            let! notes = dto.Notes |> String1k.createOption (nameof dto.Notes)
            let! restTime = dto.RestTime |> Ok
            let! sets = dto.Sets |> ExerciseSetsDto.toDomain
            let! createdOn = dto.CreatedOn |> Ok
            let! lastModifiedOn = dto.LastModifiedOn |> Ok
            let! ownerId = dto.OwnerId |> SportsmanId.create (nameof dto.OwnerId)

            return ExerciseTemplate.create id categoryId name notes restTime sets createdOn lastModifiedOn ownerId
        }
