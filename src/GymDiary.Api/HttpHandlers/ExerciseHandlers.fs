namespace GymDiary.Api.HttpHandlers

open System

open Giraffe

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
                createExercise
                    {
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
                | Error error ->
                    let message = CreateExercise.CommandError.toString error

                    match error with
                    | CreateExercise.InvalidCommand errors -> RequestErrors.BAD_REQUEST(ErrorResponse.validationErrors errors)

                    | CreateExercise.CategoryNotFound _ -> RequestErrors.CONFLICT(ErrorResponse.exerciseCategoryNotFound message)

                    | CreateExercise.OwnerNotFound _ -> RequestErrors.CONFLICT(ErrorResponse.ownerNotFound message)

            return! handler next ctx
        }
