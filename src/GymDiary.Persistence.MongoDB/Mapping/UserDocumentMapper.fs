namespace GymDiary.Persistence.MongoDB.Mapping

open System
open Common.Extensions
open FsToolkit.ErrorHandling
open FSharp.UMX
open GymDiary.Domain.Users
open GymDiary.Persistence.MongoDB.Documents
open GymDiary.Application.Validation
open GymDiary.Domain.Primitives.SharedTypes

module private GenderMapping =
    let toDto =
        function
        | Male -> GenderDto.Male
        | Female -> GenderDto.Female
        | Other -> GenderDto.Other

    let ofDto field =
        function
        | GenderDto.Male -> Ok Male
        | GenderDto.Female -> Ok Female
        | GenderDto.Other -> Ok Other
        | other -> ValidationError(field, [ $"{other} is not a valid {nameof GenderDto}" ]) |> Error

type UserDocumentMapper() =
    interface IDocumentMapper<User, UserDocument> with
        member _.MapFromDomain(domain: User) : UserDocument =
            {
                Id = %domain.Id
                Email = domain.Email.Value
                FirstName = domain.FirstName.Value
                LastName = domain.LastName.Value
                PhoneNumber = domain.PhoneNumber |> Option.map _.Value
                DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime
                Gender = domain.Gender |> Option.map GenderMapping.toDto
                CreatedOnUtc = domain.CreatedOnUtc
                UpdatedOnUtc = domain.UpdatedOnUtc
            }

        member _.MapToDomain(document: UserDocument) : Result<User, ValidationError> = result {
            let id: UserId = %document.Id
            let! email = document.Email |> Validation.checkField (nameof document.Email) EmailAddress.create
            let! firstName = document.FirstName |> Validation.checkField (nameof document.FirstName) String50.create
            let! lastName = document.LastName |> Validation.checkField (nameof document.LastName) String50.create

            let! phoneNumber =
                document.PhoneNumber
                |> Option.traverseResult (Validation.checkField (nameof document.PhoneNumber) PhoneNumber.create)

            let dateOfBirth = document.DateOfBirth |> Option.map DateOnly.FromDateTime
            let! gender = document.Gender |> Option.traverseResult (GenderMapping.ofDto (nameof document.Gender))

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
