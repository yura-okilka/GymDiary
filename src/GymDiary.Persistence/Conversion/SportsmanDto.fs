namespace GymDiary.Persistence.Conversion

open System

open GymDiary.Core.Extensions
open GymDiary.Core.Domain.Errors
open GymDiary.Core.Domain.CommonTypes
open GymDiary.Core.Domain.DomainTypes
open GymDiary.Persistence.Dtos

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module SportsmanDto =

    let fromDomain (domain: Sportsman) : SportsmanDto =
        { Id = domain.Id |> SportsmanId.value
          Email = domain.Email |> EmailAddress.value
          FirstName = domain.FirstName |> String50.value
          LastName = domain.LastName |> String50.value
          DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime |> Option.toNullable
          Sex = domain.Sex |> Option.map SexDto.fromDomain |> Option.toNullable }

    let toDomain (dto: SportsmanDto) : Result<Sportsman, ValidationError> =
        let id = dto.Id |> SportsmanId |> Ok
        let email = dto.Email |> EmailAddress.create "Email"
        let firstName = dto.FirstName |> String50.create "FirstName"
        let lastName = dto.LastName |> String50.create "LastName"
        let dateOfBirth = dto.DateOfBirth |> Option.ofNullable |> Option.map DateOnly.FromDateTime |> Ok
        let sex = dto.Sex |> Option.ofNullable |> Option.traverseResult (SexDto.toDomain "Sex")

        Sportsman.create <!> id <*> email <*> firstName <*> lastName <*> dateOfBirth <*> sex
