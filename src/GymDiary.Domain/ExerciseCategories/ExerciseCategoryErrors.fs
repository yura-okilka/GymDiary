[<AutoOpen>]
module GymDiary.Domain.ExerciseCategories.ExerciseCategoryErrors

type ExerciseCategoryNotFoundError =
    | ExerciseCategoryNotFoundError of id: string * ownerId: string

    static member toString(ExerciseCategoryNotFoundError(id, ownerId)) =
        $"Exercise category with id %s{id} and owner %s{ownerId} was not found"
