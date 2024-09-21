namespace GymDiary.Api.RouteHandlers

open System.Threading.Tasks
open GymDiary.Api
open GymDiary.Core.Workflows.ExerciseCategory
open Microsoft.AspNetCore.Http

type CreateExerciseCategoryRequest = { Name: string }
type RenameExerciseCategoryRequest = { Name: string }

type ExerciseCategoryHandler(createExerciseCategory: CreateExerciseCategory.CommandHandler) =
    member _.Create(userId: string, request: CreateExerciseCategoryRequest) : Task<IResult> = task {
        let! result =
            createExerciseCategory {
                Name = request.Name
                OwnerId = userId
            }

        return
            match result with
            | Ok data -> Results.Ok(data) // TODO: use Results.Created
            | Error(CreateExerciseCategory.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
            | Error(CreateExerciseCategory.CategoryAlreadyExists e) -> Results.Conflict(Responses.exerciseCategoryAlreadyExists e)
            | Error(CreateExerciseCategory.OwnerNotFound e) -> Results.Conflict(Responses.ownerNotFound e)
    }

    static member GetAll(getAllExerciseCategories: GetAllExerciseCategories.Workflow, userId: string) : Task<IResult> = task {
        let! result = getAllExerciseCategories { OwnerId = userId }

        return
            match result with
            | Ok data -> Results.Ok(data)
            | Error(GetAllExerciseCategories.InvalidQuery e) -> Results.BadRequest(Responses.validationError e)
    }

    static member GetById(getExerciseCategory: GetExerciseCategory.Workflow, userId: string, categoryId: string) : Task<IResult> = task {
        let! result = getExerciseCategory { Id = categoryId; OwnerId = userId }

        return
            match result with
            | Ok data -> Results.Ok(data)
            | Error(GetExerciseCategory.InvalidQuery es) -> Results.BadRequest(Responses.validationErrors es)
            | Error(GetExerciseCategory.CategoryNotFound e) -> Results.NotFound(Responses.exerciseCategoryNotFound e)
    }

    static member Rename
        (
            renameExerciseCategory: RenameExerciseCategory.Workflow,
            userId: string,
            categoryId: string,
            request: RenameExerciseCategoryRequest
        ) : Task<IResult> =
        task {
            let! result =
                renameExerciseCategory {
                    Id = categoryId
                    OwnerId = userId
                    Name = request.Name
                }

            return
                match result with
                | Ok _ -> Results.NoContent()
                | Error(RenameExerciseCategory.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
                | Error(RenameExerciseCategory.CategoryNotFound e) -> Results.NotFound(Responses.exerciseCategoryNotFound e)
                | Error(RenameExerciseCategory.NameAlreadyUsed e) -> Results.Conflict(Responses.exerciseCategoryAlreadyExists e)
        }

    static member Delete(deleteExerciseCategory: DeleteExerciseCategory.Workflow, userId: string, categoryId: string) : Task<IResult> = task {
        let! result = deleteExerciseCategory { Id = categoryId; OwnerId = userId }

        return
            match result with
            | Ok _ -> Results.NoContent()
            | Error(DeleteExerciseCategory.InvalidCommand es) -> Results.BadRequest(Responses.validationErrors es)
    }
