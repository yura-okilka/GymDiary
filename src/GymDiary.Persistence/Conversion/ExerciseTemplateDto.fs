namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseTemplateDto =

    let fromDomain (domain: ExerciseTemplate) : ExerciseTemplateDto =
        { Id = domain.Id |> Id.value
          CategoryId = domain.CategoryId |> Id.value
          Name = domain.Name |> String50.value
          Notes = domain.Notes |> Option.map String1k.value |> Option.defaultValue defaultof<string>
          RestTime = domain.RestTime
          Sets = domain.Sets |> ExerciseSetsDto.fromDomain
          CreatedOn = domain.CreatedOn
          LastModifiedOn = domain.LastModifiedOn
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (dto: ExerciseTemplateDto) : Result<ExerciseTemplate, ValidationError> =
        result {
            let! id = dto.Id |> Id.create (nameof dto.Id)
            let! categoryId = dto.CategoryId |> Id.create (nameof dto.CategoryId)
            let! name = dto.Name |> String50.create (nameof dto.Name)
            let! notes = dto.Notes |> String1k.createOption (nameof dto.Notes)
            let! sets = dto.Sets |> ExerciseSetsDto.toDomain
            let! ownerId = dto.OwnerId |> Id.create (nameof dto.OwnerId)

            return
                ExerciseTemplate.create
                    id
                    categoryId
                    name
                    notes
                    dto.RestTime
                    sets
                    dto.CreatedOn
                    dto.LastModifiedOn
                    ownerId
        }
