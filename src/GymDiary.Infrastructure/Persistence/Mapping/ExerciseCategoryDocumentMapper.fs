namespace GymDiary.Infrastructure.Persistence.Mapping

open FsToolkit.ErrorHandling
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.ExerciseCategories

type ExerciseCategoryDocumentMapper() =
    interface IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument> with
        member _.MapFromDomain(domain: ExerciseCategory) : ExerciseCategoryDocument = {
            Id = domain.Id.Value
            Name = domain.Name.Value
            OwnerId = domain.OwnerId.Value
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: ExerciseCategoryDocument) : Result<ExerciseCategory, ValidationError> = result {
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            return {
                Id = Id(document.Id)
                Name = name
                OwnerId = Id(document.OwnerId)
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
