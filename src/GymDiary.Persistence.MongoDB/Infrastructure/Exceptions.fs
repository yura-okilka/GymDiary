namespace GymDiary.Persistence.MongoDB

exception DocumentConversionException of document: string * error: GymDiary.Application.Validation.ValidationError
