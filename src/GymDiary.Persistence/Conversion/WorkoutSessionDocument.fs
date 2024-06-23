namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence
open FsToolkit.ErrorHandling

module WorkoutSessionDocument =

    let fromDomain (domain: WorkoutSession) : WorkoutSessionDocument = {
        Id = domain.Id |> Id.value
        RoutineId = domain.RoutineId |> Id.value
        Exercises = domain.Exercises |> List.map ExerciseSessionDocument.fromDomain
        StartedOn = domain.StartedOn
        CompletedOn = domain.CompletedOn
        OwnerId = domain.OwnerId |> Id.value
    }

    let toDomain (document: WorkoutSessionDocument) : Result<WorkoutSession, ValidationError> = result {
        let! id = document.Id |> Id.tryCreate (nameof document.Id)
        let! routineId = document.RoutineId |> Id.tryCreate (nameof document.RoutineId)
        let! exercises = document.Exercises |> List.traverseResultM ExerciseSessionDocument.toDomain
        let! ownerId = document.OwnerId |> Id.tryCreate (nameof document.OwnerId)

        return WorkoutSession.create id routineId exercises document.StartedOn document.CompletedOn ownerId
    }
