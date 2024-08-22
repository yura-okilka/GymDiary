namespace GymDiary.Core.Workflows.User

open System
open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator
open GymDiary.Core.Persistence

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module CreateUser =

    type GenderDto =
        | Male
        | Female
        | Other

    type Command = {
        Email: string
        FirstName: string
        LastName: string
        DateOfBirth: DateTime option
        Gender: GenderDto option
    }

    type CommandResult = { Id: string }

    type CommandError =
        | InvalidCommand of ValidationError list
        | UserAlreadyExists of UserWithEmailAlreadyExistsError

        static member userAlreadyExists email =
            UserWithEmailAlreadyExistsError.create email |> UserAlreadyExists |> Error

        static member toString error =
            match error with
            | InvalidCommand es -> es |> ValidationErrors.toString
            | UserAlreadyExists e -> e |> UserWithEmailAlreadyExistsError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.UserCreationFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) = Map [ (nameof command.Email, command.Email) ]
        }

    let execute
        (idProvider: IIdProvider)
        (userWithEmailExistsInDB: EmailAddress -> Async<bool>)
        (createUserInDB: User -> Async<UserId>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! user =
                validation {
                    let dtoToGender =
                        function
                        | GenderDto.Male -> Gender.Male
                        | GenderDto.Female -> Gender.Female
                        | GenderDto.Other -> Gender.Other

                    let id = idProvider.GenerateId()
                    let! email = EmailAddress.create (nameof command.Email) command.Email
                    and! firstName = String50.create (nameof command.FirstName) command.FirstName
                    and! lastName = String50.create (nameof command.LastName) command.LastName
                    and! dateOfBirth = command.DateOfBirth |> Option.map DateOnly.FromDateTime |> Ok
                    and! gender = command.Gender |> Option.map dtoToGender |> Ok
                    return User.create id email firstName lastName dateOfBirth gender
                }
                |> Result.mapError InvalidCommand

            let! userExists = userWithEmailExistsInDB user.Email

            if userExists then
                return! CommandError.userAlreadyExists user.Email

            let! userId = createUserInDB user |> Async.map Id.value

            logger.LogInformation(DomainEvents.UserCreated, "Exercise user was created with id '{id}'", userId)

            return { Id = userId }
        }
