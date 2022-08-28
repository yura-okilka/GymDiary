namespace GymDiary.Persistence.Conversion

open System

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling

module SportsmanDocument =

    let fromDomain (domain: Sportsman) : SportsmanDocument =
        let genderToString =
            function
            | Male -> "Male"
            | Female -> "Female"
            | Other -> "Other"

        {
            Id = domain.Id |> Id.value
            Email = domain.Email |> EmailAddress.value
            FirstName = domain.FirstName |> String50.value
            LastName = domain.LastName |> String50.value
            DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime
            Gender = domain.Gender |> Option.map genderToString
        }

    let toDomain (document: SportsmanDocument) : Result<Sportsman, ValidationError> =
        result {
            let stringToGender field gender =
                match gender with
                | "Male" -> Male |> Ok
                | "Female" -> Female |> Ok
                | "Other" -> Other |> Ok
                | _ -> ValidationError.invalidValue field gender |> Error

            let! id = document.Id |> Id.create (nameof document.Id)
            let! email = document.Email |> EmailAddress.create (nameof document.Email)
            let! firstName = document.FirstName |> String50.create (nameof document.FirstName)
            let! lastName = document.LastName |> String50.create (nameof document.LastName)
            let dateOfBirth = document.DateOfBirth |> Option.map DateOnly.FromDateTime
            let! gender = document.Gender |> Option.traverseResult (stringToGender (nameof document.Gender))

            return Sportsman.create id email firstName lastName dateOfBirth gender
        }
