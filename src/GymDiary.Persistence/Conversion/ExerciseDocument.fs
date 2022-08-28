namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module ExerciseDocument =

    let fromDomain (domain: Exercise) : ExerciseDocument =
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

    let toDomain (document: ExerciseDocument) : Result<Exercise, ValidationError> =
        result {
            let! id = document.Id |> Id.create (nameof document.Id)
            let! categoryId = document.CategoryId |> Id.create (nameof document.CategoryId)
            let! name = document.Name |> String50.create (nameof document.Name)
            let! notes = document.Notes |> Option.traverseResult (String1k.create (nameof document.Notes))
            let! sets = document.Sets |> ExerciseSetDocument.toExerciseSets document.SetsType
            let! ownerId = document.OwnerId |> Id.create (nameof document.OwnerId)

            return
                Exercise.create
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
