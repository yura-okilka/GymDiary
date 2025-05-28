namespace GymDiary.Infrastructure.Persistence.Mapping

open FsToolkit.ErrorHandling
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories

type ExerciseCategoryDocumentV2Mapper() =
    interface IDocumentMapper<ExerciseCategory, ExerciseCategoryDocumentV2> with

        member _.MapFromDomain(domain: ExerciseCategory) : ExerciseCategoryDocumentV2 = {
            Id = domain.Id.Value
            Name = domain.Name.Value
            OwnerId = domain.OwnerId.Value
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: ExerciseCategoryDocumentV2) : Result<ExerciseCategory, ValidationError> = result {
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            return {
                Id = Id(document.Id)
                Name = name
                OwnerId = Id(document.OwnerId)
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
