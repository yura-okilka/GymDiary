module GymDiary.Core.Workflows.User.CreateUser

open System
open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Persistence
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging

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

type CommandHandler(idProvider: IIdProvider, userRepository: IUserRepository, logger: ILogger) =
    interface IRequestHandler<Command, CommandResult, CommandError> with

        member _.Handle command = asyncResult {
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
                    return UserAggregate.create id email firstName lastName dateOfBirth gender
                }
                |> Result.mapError InvalidCommand

            let! userExists = userRepository.ExistWithEmail user.Email

            if userExists then
                return! CommandError.userAlreadyExists user.Email

            do! userRepository.Create user

            logger.LogInformation("User was created with id '{id}'", user.Id)

            return { Id = user.Id |> Id.value }
        }
