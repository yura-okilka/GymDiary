[<AutoOpen>]
module GymDiary.Domain.Users.UserErrors

type UserWithEmailAlreadyExistsError =
    | UserWithEmailAlreadyExistsError of email: string

    static member toString(UserWithEmailAlreadyExistsError email) = $"User with email %s{email} already exists"

