namespace GymDiary.Persistence.MongoDB.Mapping

open System
open Common.Extensions
open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
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
                Id = %domain.Id
                Email = domain.Email.Value
                FirstName = domain.FirstName.Value
                LastName = domain.LastName.Value
                PhoneNumber = domain.PhoneNumber |> Option.map _.Value
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
                | _ -> ValidationError.ofField field $"{gender} is not a valid {nameof GenderDto}" |> Error

            let id: UserId = %document.Id
            let! email = document.Email |> Validation.checkField (nameof document.Email) EmailAddress.create
            let! firstName = document.FirstName |> Validation.checkField (nameof document.FirstName) String50.create
            let! lastName = document.LastName |> Validation.checkField (nameof document.LastName) String50.create

            let! phoneNumber =
                document.PhoneNumber
                |> Option.traverseResult (Validation.checkField (nameof document.PhoneNumber) PhoneNumber.create)

            let dateOfBirth = document.DateOfBirth |> Option.map DateOnly.FromDateTime
            let! gender = document.Gender |> Option.traverseResult (dtoToGender (nameof document.Gender))

            return {
                Id = id
                Email = email
                FirstName = firstName
                LastName = lastName
                PhoneNumber = phoneNumber
                DateOfBirth = dateOfBirth
                Gender = gender
                CreatedOnUtc = document.CreatedOnUtc
                UpdatedOnUtc = document.UpdatedOnUtc
            }
        }
