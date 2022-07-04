namespace GymDiary.Persistence

open GymDiary.Core.Domain

exception DocumentConversionException of document: string * error: ValidationError
