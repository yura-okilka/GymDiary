namespace GymDiary.Core.Domain

open System
open System.ComponentModel.DataAnnotations

[<AutoOpen>]
module CommonTypes =

    /// Constrained to be a strongly typed id of entity, not null
    type Id<'T> = Id of string

    module Id =

        let Empty = Id ""

        let create<'T> fieldName value : Result<Id<'T>, ValidationError> = ConstrainedType.createStringNotNull fieldName Id value

        let value (Id value) = value

    /// Constrained to be 50 chars or less, not null
    type String50 =
        private
        | String50 of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String50 (1, 50) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String50 50 value

        static member value(String50 value) = value

    /// Constrained to be 200 chars or less, not null
    type String200 =
        private
        | String200 of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String200 (1, 200) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String200 200 value

        static member value(String200 value) = value

    /// Constrained to be 1000 chars or less, not null
    type String1k =
        private
        | String1k of string

        static member create fieldName value =
            ConstrainedType.createString fieldName String1k (1, 1000) value

        static member createOption fieldName value =
            ConstrainedType.createStringOption fieldName String1k 1000 value

        static member value(String1k value) = value

    /// Constrained to be a non-zero positive natural number
    type PositiveInt =
        private
        | PositiveInt of int

        static member create fieldName value =
            ConstrainedType.createInt fieldName PositiveInt (1, Int32.MaxValue) value

        static member value(PositiveInt value) = value

    /// Constrained to be a valid email address
    type EmailAddress =
        private
        | EmailAddress of string

        static member create (fieldName: string) (value: string) =
            let attribute = new EmailAddressAttribute()

            if attribute.IsValid(value) then
                EmailAddress value |> Ok
            else
                ValidationError.invalidEmailAddress fieldName |> Error

        static member value(EmailAddress value) = value

    /// Constrained to be a valid phone number
    type PhoneNumber =
        private
        | PhoneNumber of string

        static member create (fieldName: string) (value: string) =
            let attribute = new PhoneAttribute()

            if attribute.IsValid(value) then
                PhoneNumber value |> Ok
            else
                ValidationError.invalidPhoneNumber fieldName |> Error

        static member value(PhoneNumber value) = value

    type Gender =
        | Male
        | Female
        | Other
