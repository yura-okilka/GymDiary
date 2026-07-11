namespace GymDiary.Persistence.MongoDB.Mapping

open GymDiary.Application.Workflows.Validation

type IDocumentMapper<'TEntity, 'TDocument> =
    abstract member MapFromDomain: 'TEntity -> 'TDocument
    abstract member MapToDomain: 'TDocument -> Result<'TEntity, ValidationErrors>
