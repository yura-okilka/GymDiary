namespace GymDiary.Core.Domain

open GymDiary.Core.Domain.ExerciseCategoryAggregate
open GymDiary.Core.Domain.UserAggregate

[<AutoOpen>]
module DomainErrors =

    type ExerciseCategoryNotFoundError =
        | ExerciseCategoryNotFoundError of id: string * ownerId: string

        static member create (id: ExerciseCategoryId) (ownerId: UserId) =
            ExerciseCategoryNotFoundError(id |> Id.value, ownerId |> Id.value)

        static member toString(ExerciseCategoryNotFoundError(id, ownerId)) =
            $"Exercise category with id %s{id} and owner %s{ownerId} is not found"

    type ExerciseCategoryAlreadyExistsError =
        | ExerciseCategoryAlreadyExistsError of name: string

        static member create(name: String50) = ExerciseCategoryAlreadyExistsError(name |> String50.value)

        static member toString(ExerciseCategoryAlreadyExistsError name) = $"Exercise category with name %s{name} already exists"

    type UserWithEmailAlreadyExistsError =
        | UserWithEmailAlreadyExistsError of email: string

        static member create(email: EmailAddress) =
            UserWithEmailAlreadyExistsError(email |> EmailAddress.value)

        static member toString(UserWithEmailAlreadyExistsError email) = $"User with email %s{email} already exists"

    type OwnerNotFoundError = // TODO: User
        | OwnerNotFoundError of id: string

        static member create(id: UserId) = OwnerNotFoundError(id |> Id.value)

        static member toString(OwnerNotFoundError id) = $"Owner with id %s{id} is not found"
