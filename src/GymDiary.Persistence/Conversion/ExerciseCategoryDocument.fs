namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseCategoryDocument =

    let fromDomain (domain: ExerciseCategory) : ExerciseCategoryDocument =
        { Id = domain.Id |> Id.value
          Name = domain.Name |> String50.value
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (document: ExerciseCategoryDocument) : Result<ExerciseCategory, ValidationError> =
        result {
            let! id = document.Id |> Id.create (nameof document.Id)
            let! name = document.Name |> String50.create (nameof document.Name)
            let! ownerId = document.OwnerId |> Id.create (nameof document.OwnerId)

            return ExerciseCategory.create id name ownerId
        }
