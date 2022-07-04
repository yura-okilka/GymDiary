namespace GymDiary.Persistence.Conversion

open System

open Common.Extensions

open GymDiary.Core.Domain
open GymDiary.Persistence

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module SportsmanDocument =

    let fromDomain (domain: Sportsman) : SportsmanDocument =
        { Id = domain.Id |> Id.value
          Email = domain.Email |> EmailAddress.value
          FirstName = domain.FirstName |> String50.value
          LastName = domain.LastName |> String50.value
          DateOfBirth = domain.DateOfBirth |> Option.map DateOnly.toDateTime |> Option.toNullable
          Sex = domain.Sex |> Option.map SexDocument.fromDomain |> Option.toNullable }

    let toDomain (dto: SportsmanDocument) : Result<Sportsman, ValidationError> =
        let id = dto.Id |> Id.create (nameof dto.Id)
        let email = dto.Email |> EmailAddress.create (nameof dto.Email)
        let firstName = dto.FirstName |> String50.create (nameof dto.FirstName)
        let lastName = dto.LastName |> String50.create (nameof dto.LastName)
        let dateOfBirth = dto.DateOfBirth |> Option.ofNullable |> Option.map DateOnly.FromDateTime |> Ok
        let sex = dto.Sex |> Option.ofNullable |> Option.traverseResult (SexDocument.toDomain (nameof dto.Sex))

        Sportsman.create <!> id <*> email <*> firstName <*> lastName <*> dateOfBirth <*> sex