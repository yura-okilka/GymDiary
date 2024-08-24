namespace GymDiary.Persistence.Documents

open GymDiary.Core.Domain
open GymDiary.Core.Domain.ExerciseCategoryAggregate
open FsToolkit.ErrorHandling

[<CLIMutable>]
type ExerciseCategoryDocument = {
    Id: string
    Name: string
    OwnerId: string
} with

    static member fromDomain(domain: ExerciseCategory) : ExerciseCategoryDocument = {
        Id = domain.Id |> Id.value
        Name = domain.Name |> String50.value
        OwnerId = domain.OwnerId |> Id.value
    }

    static member toDomain(document: ExerciseCategoryDocument) : Result<ExerciseCategory, ValidationError> = result {
        let! id = document.Id |> Id.tryCreate (nameof document.Id)
        let! name = document.Name |> String50.create (nameof document.Name)
        let! ownerId = document.OwnerId |> Id.tryCreate (nameof document.OwnerId)

        return ExerciseCategoryAggregate.create id name ownerId
    }
