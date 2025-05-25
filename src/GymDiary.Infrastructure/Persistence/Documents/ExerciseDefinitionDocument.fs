namespace GymDiary.Infrastructure.Persistence.Documents

open System
open GymDiary.Core.Domain
open GymDiary.Core.Domain.ExerciseDefinitionAggregate
open FsToolkit.ErrorHandling

[<CLIMutable>]
type ExerciseDefinitionDocument = {
    Id: string
    CategoryId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetsDto
    CreatedOn: DateTime
    LastModifiedOn: DateTime
    OwnerId: string
} with

    static member fromDomain(domain: ExerciseDefinition) : ExerciseDefinitionDocument = {
        Id = domain.Id |> Id.value
        CategoryId = domain.CategoryId |> Id.value
        Name = domain.Name |> String50.value
        Notes = domain.Notes |> Option.map String1k.value
        RestTime = domain.RestTime
        Sets = domain.Sets |> ExerciseSetsDto.fromDomain
        CreatedOn = domain.CreatedOn
        LastModifiedOn = domain.LastModifiedOn
        OwnerId = domain.OwnerId |> Id.value
    }

    static member toDomain(document: ExerciseDefinitionDocument) : Result<ExerciseDefinition, ValidationError> = result {
        let! id = document.Id |> Id.tryCreate (nameof document.Id)
        let! categoryId = document.CategoryId |> Id.tryCreate (nameof document.CategoryId)
        let! name = document.Name |> String50.create (nameof document.Name)
        let! notes = document.Notes |> Option.traverseResult (String1k.create (nameof document.Notes))
        let! sets = document.Sets |> ExerciseSetsDto.toDomain
        let! ownerId = document.OwnerId |> Id.tryCreate (nameof document.OwnerId)

        return {
            Id = id
            CategoryId = categoryId
            Name = name
            Notes = notes
            RestTime = document.RestTime
            Sets = sets
            CreatedOn = document.CreatedOn
            LastModifiedOn = document.LastModifiedOn
            OwnerId = ownerId
        }
    }
