namespace GymDiary.Persistence.MongoDB

exception DocumentConversionException of document: string * error: GymDiary.Application.Workflows.Validation.ValidationError
