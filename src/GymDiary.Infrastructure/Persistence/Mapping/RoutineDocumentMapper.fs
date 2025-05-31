namespace GymDiary.Infrastructure.Persistence.Mapping

open FsToolkit.ErrorHandling
open GymDiary.Domain.Routines
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type RoutineDocumentMapper() =
    interface IDocumentMapper<Routine, RoutineDocument> with

        member _.MapFromDomain(domain: Routine) : RoutineDocument = {
            Id = domain.Id.Value
            Name = domain.Name.Value
            Goal = domain.Goal |> Option.map _.Value
            Notes = domain.Notes |> Option.map _.Value
            Schedule = domain.Schedule
            ExerciseIds = domain.Exercises |> Set.map _.Value
            OwnerId = domain.OwnerId.Value
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: RoutineDocument) : Result<Routine, ValidationError> = result {
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            let! goal =
                document.Goal
                |> Option.traverseResult (Validation.checkField (nameof document.Goal) String200.create)

            let! notes =
                document.Notes
                |> Option.traverseResult (Validation.checkField (nameof document.Notes) String1k.create)

            let exercises = document.ExerciseIds |> Set.map Id

            return {
                Id = Id(document.Id)
                Name = name
                Goal = goal
                Notes = notes
                Schedule = document.Schedule
                Exercises = exercises
                OwnerId = Id(document.OwnerId)
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
