[<AutoOpen>]
module GymDiary.Domain.Users.UserErrors

type UserWithEmailAlreadyExistsError =
    | UserWithEmailAlreadyExistsError of email: string

    static member toString(UserWithEmailAlreadyExistsError email) = $"User with email %s{email} already exists"

type UserNotFoundError =
    | UserNotFoundError of id: string

    static member toString(UserNotFoundError id) = $"User with id %s{id} is not found"
