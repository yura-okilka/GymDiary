namespace GymDiary.Persistence.MongoDB.Mapping

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Domain.ExerciseCategories
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type ExerciseDefinitionDocumentMapper(setsMapper: IDocumentMapper<ExerciseSetGroup, ExerciseSetDto list>) =
    interface IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument> with
        member _.MapFromDomain(domain: ExerciseDefinition) : ExerciseDefinitionDocument = {
            Id = %domain.Id
            CategoryId = %domain.CategoryId
            Name = domain.Name.Value
            Notes = domain.Notes |> Option.map _.Value
            RestTime = domain.RestTime
            Sets = setsMapper.MapFromDomain domain.Sets
            OwnerId = %domain.OwnerId
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: ExerciseDefinitionDocument) : Result<ExerciseDefinition, ValidationErrors> = result {
            let id: ExerciseDefinitionId = %document.Id
            let categoryId: ExerciseCategoryId = %document.CategoryId
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            let! notes =
                document.Notes
                |> Option.traverseResult (Validation.checkField (nameof document.Notes) String1k.create)

            let! sets = setsMapper.MapToDomain document.Sets
            let ownerId: UserId = %document.OwnerId

            return {
                Id = id
                CategoryId = categoryId
                Name = name
                Notes = notes
                RestTime = document.RestTime
                Sets = sets
                OwnerId = ownerId
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
