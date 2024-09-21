namespace GymDiary.Api.RouteHandlers

open System
open System.Threading.Tasks
open GymDiary.Core.Workflows.CommonDtos
open Microsoft.AspNetCore.Http

type CreateExerciseRequest = {
    CategoryId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetsDto
}

type ExerciseHandler() =
    static member Create
        (
            createExercise: CreateExerciseDefinition.Workflow,
            userId: string,
            request: CreateExerciseRequest
        ) : Task<IResult> =
        task {
            let! result =
                createExercise {
                    OwnerId = userId
                    CategoryId = request.CategoryId
                    Name = request.Name
                    Notes = request.Notes
                    RestTime = request.RestTime
                    Sets = request.Sets
                }

            return
                match result with
                | Ok data -> Results.Ok(data) // TODO: use Results.Created
                | Error(CreateExerciseDefinition.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
                | Error(CreateExerciseDefinition.CategoryNotFound e) -> Results.Conflict(Responses.exerciseCategoryNotFound e)
                | Error(CreateExerciseDefinition.OwnerNotFound e) -> Results.Conflict(Responses.ownerNotFound e)
        }
