namespace GymDiary.Persistence.Conversion

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module RoutineDocument =

    let fromDomain (domain: Routine) : RoutineDocument = {
        Id = domain.Id |> Id.value
        Name = domain.Name |> String50.value
        Goal = domain.Goal |> Option.map String200.value
        Notes = domain.Notes |> Option.map String1k.value
        Schedule = domain.Schedule |> List.ofSeq
        Exercises = domain.Exercises |> List.map ExerciseDocument.fromDomain
        CreatedOn = domain.CreatedOn
        LastModifiedOn = domain.LastModifiedOn
        OwnerId = domain.OwnerId |> Id.value
    }

    let toDomain (document: RoutineDocument) : Result<Routine, ValidationError> = result {
        let! id = document.Id |> Id.create (nameof document.Id)
        let! name = document.Name |> String50.create (nameof document.Name)
        let! goal = document.Goal |> Option.traverseResult (String200.create (nameof document.Goal))
        let! notes = document.Notes |> Option.traverseResult (String1k.create (nameof document.Notes))
        let schedule = document.Schedule |> Set.ofSeq
        let! exercises = document.Exercises |> List.traverseResultM ExerciseDocument.toDomain
        let! ownerId = document.OwnerId |> Id.create (nameof document.OwnerId)

        return Routine.create id name goal notes schedule exercises document.CreatedOn document.LastModifiedOn ownerId
    }
