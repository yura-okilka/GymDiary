namespace GymDiary.Persistence.MongoDB.Mapping

open FsToolkit.ErrorHandling
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type ExerciseDefinitionDocumentMapper(setsMapper: IDocumentMapper<ExerciseSets, ExerciseSetDto list>) =
    interface IDocumentMapper<ExerciseDefinition, ExerciseDefinitionDocument> with
        member _.MapFromDomain(domain: ExerciseDefinition) : ExerciseDefinitionDocument = {
            Id = domain.Id.Value
            CategoryId = domain.CategoryId.Value
            Name = domain.Name.Value
            Notes = domain.Notes |> Option.map _.Value
            RestTime = domain.RestTime
            Sets = setsMapper.MapFromDomain domain.Sets
            OwnerId = domain.OwnerId.Value
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: ExerciseDefinitionDocument) : Result<ExerciseDefinition, ValidationError> = result {
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            let! notes =
                document.Notes
                |> Option.traverseResult (Validation.checkField (nameof document.Notes) String1k.create)

            let! sets = setsMapper.MapToDomain document.Sets

            return {
                Id = Id(document.Id)
                CategoryId = Id(document.CategoryId)
                Name = name
                Notes = notes
                RestTime = document.RestTime
                Sets = sets
                OwnerId = Id(document.OwnerId)
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
