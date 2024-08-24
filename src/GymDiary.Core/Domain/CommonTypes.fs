namespace GymDiary.Core.Domain

open System
open System.ComponentModel.DataAnnotations

[<AutoOpen>]
module CommonTypes =

    /// Constrained to be a strongly typed id of entity, not null
    [<Struct>]
    type Id<'T> = Id of string

    module Id =

        let create<'T> value : Id<'T> = Id value

        let tryCreate<'T> fieldName value : Result<Id<'T>, ValidationError> = ConstrainedType.createStringNotNull fieldName Id value // TODO: return narrow error

        let value (Id value) = value

    /// Constrained to be 50 chars or less, not null
    [<Struct>]
    type String50 =
        private
        | String50 of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String50 (1, 50) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String50 50 value

        static member value(String50 value) = value

    /// Constrained to be 200 chars or less, not null
    [<Struct>]
    type String200 =
        private
        | String200 of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String200 (1, 200) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String200 200 value

        static member value(String200 value) = value

    /// Constrained to be 1000 chars or less, not null
    [<Struct>]
    type String1k =
        private
        | String1k of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String1k (1, 1000) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String1k 1000 value

        static member value(String1k value) = value

    /// Constrained to be a non-zero positive natural number
    [<Struct>]
    type PositiveInt =
        private
        | PositiveInt of int

        static member create fieldName value =
            ConstrainedType.createInt fieldName PositiveInt (1, Int32.MaxValue) value

        static member value(PositiveInt value) = value

    /// Constrained to be a valid email address
    [<Struct>]
    type EmailAddress =
        private
        | EmailAddress of string

        static member create (fieldName: string) (value: string) =

            if EmailAddressAttribute().IsValid(value) then
                EmailAddress value |> Ok
            else
                ValidationError.invalidEmailAddress fieldName |> Error

        static member value(EmailAddress value) = value

    /// Constrained to be a valid phone number
    [<Struct>]
    type PhoneNumber =
        private
        | PhoneNumber of string

        static member create (fieldName: string) (value: string) =

            if PhoneAttribute().IsValid(value) then
                PhoneNumber value |> Ok
            else
                ValidationError.invalidPhoneNumber fieldName |> Error

        static member value(PhoneNumber value) = value

    type Gender =
        | Male
        | Female
        | Other
