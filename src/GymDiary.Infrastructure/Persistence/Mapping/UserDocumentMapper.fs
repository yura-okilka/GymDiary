namespace GymDiary.Infrastructure.Persistence.Mapping

open System
open Common.Extensions
open FsToolkit.ErrorHandling
open GymDiary.Domain.Users
open GymDiary.Infrastructure.Persistence.Documents
open GymDiary.Application.Workflows.Validation
open GymDiary.Domain.Primitives.SharedTypes

type UserDocumentMapper() =
    interface IDocumentMapper<User, UserDocument> with

        member _.MapFromDomain(domain: User) : UserDocument =
            let genderToDto =
                function
                | Male -> GenderDto.Male
                | Female -> GenderDto.Female
                | Other -> GenderDto.Other

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

        member _.MapToDomain(document: UserDocument) : Result<User, ValidationError> = result {
            let dtoToGender field gender =
                match gender with
                | GenderDto.Male -> Male |> Ok
                | GenderDto.Female -> Female |> Ok
                | GenderDto.Other -> Other |> Ok
                | _ -> ValidationError(field, $"{gender} is not a valid {nameof GenderDto}") |> Error

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
