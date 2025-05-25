namespace GymDiary.Persistence.Documents

open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.ExerciseCategories
open FsToolkit.ErrorHandling
open GymDiary.Domain.Primitives.SharedTypes

[<CLIMutable>]
type ExerciseCategoryDocumentV2 = {
    Id: string
    Name: string
    OwnerId: string
} with

    static member fromDomain(domain: ExerciseCategory) : ExerciseCategoryDocumentV2 = {
        Id = domain.Id.Value
        Name = domain.Name.Value
        OwnerId = domain.OwnerId.Value
    }

    static member toDomain(document: ExerciseCategoryDocumentV2) : Result<ExerciseCategory, ValidationError> = result {
        let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

        return {
            Id = Id(document.Id)
            Name = name
            OwnerId = Id(document.OwnerId)
        }
    }
