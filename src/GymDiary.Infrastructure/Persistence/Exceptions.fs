namespace GymDiary.Infrastructure.Persistence

open GymDiary.Core.Domain

exception DocumentConversionException of document: string * error: ValidationError
exception DocumentConversionExceptionV2 of document: string * error: GymDiary.Application.Workflows.Validation.ValidationError
