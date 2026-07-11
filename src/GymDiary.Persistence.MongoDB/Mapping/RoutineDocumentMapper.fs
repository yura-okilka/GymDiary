namespace GymDiary.Persistence.MongoDB.Mapping

open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Domain.ExerciseDefinitions
open GymDiary.Domain.Routines
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type RoutineDocumentMapper() =
    interface IDocumentMapper<Routine, RoutineDocument> with
        member _.MapFromDomain(domain: Routine) : RoutineDocument = {
            Id = %domain.Id
            Name = domain.Name.Value
            Goal = domain.Goal |> Option.map _.Value
            Notes = domain.Notes |> Option.map _.Value
            Schedule = domain.Schedule
            ExerciseIds = domain.Exercises |> Set.map (fun id -> %id)
            OwnerId = %domain.OwnerId
            CreatedOnUtc = domain.CreatedOnUtc
            UpdatedOnUtc = domain.UpdatedOnUtc
        }

        member _.MapToDomain(document: RoutineDocument) : Result<Routine, ValidationErrors> = result {
            let id: RoutineId = %document.Id
            let! name = document.Name |> Validation.checkField (nameof document.Name) String50.create

            let! goal =
                document.Goal
                |> Option.traverseResult (Validation.checkField (nameof document.Goal) String200.create)

            let! notes =
                document.Notes
                |> Option.traverseResult (Validation.checkField (nameof document.Notes) String1k.create)

            let exercises: ExerciseDefinitionId Set = document.ExerciseIds |> Set.map (fun id -> %id)
            let ownerId: UserId = %document.OwnerId

            return {
                Id = id
                Name = name
                Goal = goal
                Notes = notes
                Schedule = document.Schedule
                Exercises = exercises
                OwnerId = ownerId
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
