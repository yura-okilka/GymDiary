namespace GymDiary.Persistence.Documents

open System
open Common.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Domain.UserAggregate
open FsToolkit.ErrorHandling

type GenderDto =
    | Male = 1
    | Female = 2
    | Other = 3

[<CLIMutable>]
type UserDocument = {
    Id: string
    Email: string
    FirstName: string
    LastName: string
    DateOfBirth: DateTime option
    Gender: GenderDto option
} with

    static member fromDomain(domain: User) : UserDocument =
        let genderToDto =
            function
            | Male -> GenderDto.Male
            | Female -> GenderDto.Female
            | Other -> GenderDto.Other

        {
            Id = domain.Id |> Id.value
            Email = domain.Email |> EmailAddress.value
            FirstName = domain.FirstName |> String50.value
            LastName = domain.LastName |> String50.value
            DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime
            Gender = domain.Gender |> Option.map genderToDto
        }

    static member toDomain(document: UserDocument) : Result<User, ValidationError> = result {
        let dtoToGender field gender =
            match gender with
            | GenderDto.Male -> Male |> Ok
            | GenderDto.Female -> Female |> Ok
            | GenderDto.Other -> Other |> Ok
            | _ -> ValidationError.invalidValue field (nameof gender) |> Error

        let id = document.Id |> Id.create
        let! email = document.Email |> EmailAddress.create (nameof document.Email)
        let! firstName = document.FirstName |> String50.create (nameof document.FirstName)
        let! lastName = document.LastName |> String50.create (nameof document.LastName)
        let dateOfBirth = document.DateOfBirth |> Option.map DateOnly.FromDateTime
        let! gender = document.Gender |> Option.traverseResult (dtoToGender (nameof document.Gender))

        return {
            Id = id
            Email = email
            FirstName = firstName
            LastName = lastName
            DateOfBirth = dateOfBirth
            Gender = gender
        }
    }
