namespace GymDiary.Infrastructure.Persistence

exception DocumentConversionException of document: string * error: GymDiary.Application.Workflows.Validation.ValidationError
