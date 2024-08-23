namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Core.Domain.ExerciseDefinitionAggregate
open GymDiary.Persistence
open FsToolkit.ErrorHandling

module ExerciseDefinitionDocument =

    let fromDomain (domain: ExerciseDefinition) : ExerciseDefinitionDocument =
        let (setType, sets) = ExerciseSetDocument.fromExerciseSets domain.Sets

        {
            Id = domain.Id |> Id.value
            CategoryId = domain.CategoryId |> Id.value
            Name = domain.Name |> String50.value
            Notes = domain.Notes |> Option.map String1k.value
            RestTime = domain.RestTime
            SetsType = setType
            Sets = sets
            CreatedOn = domain.CreatedOn
            LastModifiedOn = domain.LastModifiedOn
            OwnerId = domain.OwnerId |> Id.value
        }

    let toDomain (document: ExerciseDefinitionDocument) : Result<ExerciseDefinition, ValidationError> = result {
        let! id = document.Id |> Id.tryCreate (nameof document.Id)
        let! categoryId = document.CategoryId |> Id.tryCreate (nameof document.CategoryId)
        let! name = document.Name |> String50.create (nameof document.Name)
        let! notes = document.Notes |> Option.traverseResult (String1k.create (nameof document.Notes))
        let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType
        let! ownerId = document.OwnerId |> Id.tryCreate (nameof document.OwnerId)

        return
            ExerciseDefinitionAggregate.restoreFrom
                id
                categoryId
                name
                notes
                document.RestTime
                sets
                document.CreatedOn
                document.LastModifiedOn
                ownerId
    }
