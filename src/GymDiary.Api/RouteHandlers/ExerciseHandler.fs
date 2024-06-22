namespace GymDiary.Api.RouteHandlers

open System
open System.Threading.Tasks
open GymDiary.Api
open GymDiary.Core.Workflows.CommonDtos
open GymDiary.Core.Workflows.Exercise
open Microsoft.AspNetCore.Http

type CreateExerciseRequest = {
    CategoryId: string
    Name: string
    Notes: string option
    RestTime: TimeSpan
    Sets: ExerciseSetsDto
}

type ExerciseHandler() =
    static member Create(createExercise: CreateExercise.Workflow, userId: string, request: CreateExerciseRequest) : Task<IResult> = task {
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
            | Error(CreateExercise.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
            | Error(CreateExercise.CategoryNotFound e) -> Results.Conflict(Responses.exerciseCategoryNotFound e)
            | Error(CreateExercise.OwnerNotFound e) -> Results.Conflict(Responses.ownerNotFound e)
    }
