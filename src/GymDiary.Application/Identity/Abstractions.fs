namespace GymDiary.Application.Identity

/// The user on whose behalf the current operation is executing.
/// Authentication is enforced at the edge, so a resolved ICurrentUser is always authenticated.
type ICurrentUser =
    abstract member Id: string
    abstract member Email: string
