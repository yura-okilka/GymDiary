namespace GymDiary.Core.Workflows.Sportsman

open System
open GymDiary.Core.Domain
open GymDiary.Core.Workflows
open GymDiary.Core.Workflows.ErrorLoggingDecorator

open FsToolkit.ErrorHandling

open Microsoft.Extensions.Logging

module CreateSportsman =

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
        | SportsmanAlreadyExists of SportsmanWithEmailAlreadyExistsError

        static member sportsmanAlreadyExists email =
            SportsmanWithEmailAlreadyExistsError.create email |> SportsmanAlreadyExists |> Error

        static member toString error =
            match error with
            | InvalidCommand es -> es |> ValidationErrors.toString
            | SportsmanAlreadyExists e -> e |> SportsmanWithEmailAlreadyExistsError.toString

    type Workflow = Workflow<Command, CommandResult, CommandError>

    let LoggingInfoProvider =
        { new ILoggingInfoProvider<Command, CommandError> with

            member _.ErrorEventId = DomainEvents.SportsmanCreationFailed

            member _.GetErrorMessage(error) = CommandError.toString error

            member _.GetRequestInfo(command) = Map [ (nameof command.Email, command.Email) ]
        }

    let execute
        (sportsmanWithEmailExistsInDB: EmailAddress -> Async<bool>)
        (createSportsmanInDB: Sportsman -> Async<SportsmanId>)
        (logger: ILogger)
        (command: Command)
        =
        asyncResult {
            let! sportsman =
                validation {
                    let dtoToGender =
                        function
                        | GenderDto.Male -> Gender.Male
                        | GenderDto.Female -> Gender.Female
                        | GenderDto.Other -> Gender.Other

                    let! email = EmailAddress.create (nameof command.Email) command.Email
                    and! firstName = String50.create (nameof command.FirstName) command.FirstName
                    and! lastName = String50.create (nameof command.LastName) command.LastName
                    and! dateOfBirth = command.DateOfBirth |> Option.map DateOnly.FromDateTime |> Ok
                    and! gender = command.Gender |> Option.map dtoToGender |> Ok
                    return Sportsman.create email firstName lastName dateOfBirth gender
                }
                |> Result.mapError InvalidCommand

            let! sportsmanExists = sportsmanWithEmailExistsInDB sportsman.Email

            if sportsmanExists then
                return! CommandError.sportsmanAlreadyExists sportsman.Email

            let! sportsmanId = createSportsmanInDB sportsman |> Async.map Id.value

            logger.LogInformation(DomainEvents.SportsmanCreated, "Exercise sportsman was created with id '{id}'", sportsmanId)

            return { Id = sportsmanId }
        }
