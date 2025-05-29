namespace GymDiary.Infrastructure.Persistence.Mapping

open System
open Common.Extensions
open FsToolkit.ErrorHandling
open GymDiary.Domain.Users
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type UserDocumentV2Mapper() =
    interface IDocumentMapper<User, UserDocumentV2> with

        member _.MapFromDomain(domain: User) : UserDocumentV2 =
            let genderToDto =
                function
                | Male -> GenderDtoV2.Male
                | Female -> GenderDtoV2.Female
                | Other -> GenderDtoV2.Other

            {
                Id = domain.Id.Value
                Email = domain.Email.Value
                FirstName = domain.FirstName.Value
                LastName = domain.LastName.Value
                DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime
                Gender = domain.Gender |> Option.map genderToDto
                CreatedOnUtc = domain.CreatedOnUtc
                UpdatedOnUtc = domain.UpdatedOnUtc
            }

        member _.MapToDomain(document: UserDocumentV2) : Result<User, ValidationError> = result {
            let dtoToGender field gender =
                match gender with
                | GenderDtoV2.Male -> Male |> Ok
                | GenderDtoV2.Female -> Female |> Ok
                | GenderDtoV2.Other -> Other |> Ok
                | _ -> ValidationError(field, $"{gender} is not a valid {nameof GenderDtoV2}") |> Error

            let! email = document.Email |> Validation.checkField (nameof document.Email) EmailAddress.create
            let! firstName = document.FirstName |> Validation.checkField (nameof document.FirstName) String50.create
            let! lastName = document.LastName |> Validation.checkField (nameof document.LastName) String50.create
            let dateOfBirth = document.DateOfBirth |> Option.map DateOnly.FromDateTime
            let! gender = document.Gender |> Option.traverseResult (dtoToGender (nameof document.Gender))

            return {
                Id = Id(document.Id)
                Email = email
                FirstName = firstName
                LastName = lastName
                DateOfBirth = dateOfBirth
                Gender = gender
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
