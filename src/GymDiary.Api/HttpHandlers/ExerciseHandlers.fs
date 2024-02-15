namespace GymDiary.Api.HttpHandlers

open System

open Giraffe

open GymDiary.Api
open GymDiary.Core.Workflows.CommonDtos
open GymDiary.Core.Workflows.Exercise

open Microsoft.AspNetCore.Http

module ExerciseHandlers =

    type CreateExerciseRequest = {
        CategoryId: string
        Name: string
        Notes: string option
        RestTime: TimeSpan
        Sets: ExerciseSetsDto
    }

    let create (createExercise: CreateExercise.Workflow) (sportsmanId: string) (request: CreateExerciseRequest) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                createExercise {
                    OwnerId = sportsmanId
                    CategoryId = request.CategoryId
                    Name = request.Name
                    Notes = request.Notes
                    RestTime = request.RestTime
                    Sets = request.Sets
                }

            let handler =
                match result with
                | Ok data -> Successful.CREATED data
                | Error(CreateExercise.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(CreateExercise.CategoryNotFound e) -> RequestErrors.CONFLICT(Responses.exerciseCategoryNotFound e)
                | Error(CreateExercise.OwnerNotFound e) -> RequestErrors.CONFLICT(Responses.ownerNotFound e)

            return! handler next ctx
        }
