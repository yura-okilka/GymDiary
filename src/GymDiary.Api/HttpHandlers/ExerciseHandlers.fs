namespace GymDiary.Api.HttpHandlers

open System

open Giraffe

open GymDiary.Core.Workflows.CommonDtos

open Microsoft.AspNetCore.Http

module ExerciseHandlers =

    type CreateExerciseRequest = {
        CategoryId: string
        Name: string
        Notes: string option
        RestTime: TimeSpan
        Sets: ExerciseSetsDto
    }

    let create (createExercise: CreateExerciseDefinition.Workflow) (userId: string) (request: CreateExerciseRequest) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let! result =
                createExercise {
                    OwnerId = userId
                    CategoryId = request.CategoryId
                    Name = request.Name
                    Notes = request.Notes
                    RestTime = request.RestTime
                    Sets = request.Sets
                }

            let handler =
                match result with
                | Ok data -> Successful.CREATED data
                | Error(CreateExerciseDefinition.InvalidCommand es) -> RequestErrors.BAD_REQUEST(Responses.validationErrors es)
                | Error(CreateExerciseDefinition.CategoryNotFound e) -> RequestErrors.CONFLICT(Responses.exerciseCategoryNotFound e)
                | Error(CreateExerciseDefinition.OwnerNotFound e) -> RequestErrors.CONFLICT(Responses.ownerNotFound e)

            return! handler next ctx
        }
