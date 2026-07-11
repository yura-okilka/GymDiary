namespace GymDiary.Persistence.MongoDB.Mapping

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Validation
open GymDiary.Domain.Primitives.SharedTypes
open GymDiary.Domain.Users
open GymDiary.Domain.ExerciseCategories

type ExerciseCategoryDocumentMapper() =
    interface IDocumentMapper<ExerciseCategory, ExerciseCategoryDocument> with
        member _.MapFromDomain(domain: ExerciseCategory) : ExerciseCategoryDocument = {
            Id = %domain.Id
            Name = domain.Name.Value
            OwnerId = %domain.OwnerId
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: ExerciseCategoryDocument) : Result<ExerciseCategory, ValidationError> = result {
            let id: ExerciseCategoryId = %document.Id
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create
            let ownerId: UserId = %document.OwnerId

            return {
                Id = id
                Name = name
                OwnerId = ownerId
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
