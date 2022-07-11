namespace GymDiary.Persistence.Conversion

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module WorkoutSessionDocument =

    let fromDomain (domain: WorkoutSession) : WorkoutSessionDocument =
        { Id = domain.Id |> Id.value
          TemplateId = domain.TemplateId |> Id.value
          Exercises = domain.Exercises |> List.map ExerciseDocument.fromDomain
          StartedOn = domain.StartedOn
          CompletedOn = domain.CompletedOn
          OwnerId = domain.OwnerId |> Id.value }

    let toDomain (document: WorkoutSessionDocument) : Result<WorkoutSession, ValidationError> =
        result {
            let! id = document.Id |> Id.create (nameof document.Id)
            let! templateId = document.TemplateId |> Id.create (nameof document.TemplateId)
            let! exercises = document.Exercises |> List.traverseResultM ExerciseDocument.toDomain
            let! ownerId = document.OwnerId |> Id.create (nameof document.OwnerId)

            return WorkoutSession.create id templateId exercises document.StartedOn document.CompletedOn ownerId
        }
