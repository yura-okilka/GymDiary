namespace GymDiary.Persistence.Conversion

open System

open GymDiary.Core.Extensions
open GymDiary.Core.Domain
open GymDiary.Core.Domain.Logic
open GymDiary.Persistence

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
        let id = dto.Id |> SportsmanId.create (nameof dto.Id)
        let email = dto.Email |> EmailAddress.create (nameof dto.Email)
        let firstName = dto.FirstName |> String50.create (nameof dto.FirstName)
        let lastName = dto.LastName |> String50.create (nameof dto.LastName)
        let dateOfBirth = dto.DateOfBirth |> Option.ofNullable |> Option.map DateOnly.FromDateTime |> Ok
        let sex = dto.Sex |> Option.ofNullable |> Option.traverseResult (SexDto.toDomain (nameof dto.Sex))

        Sportsman.create <!> id <*> email <*> firstName <*> lastName <*> dateOfBirth <*> sex
