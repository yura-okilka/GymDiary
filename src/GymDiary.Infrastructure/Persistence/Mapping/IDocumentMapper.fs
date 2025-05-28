namespace GymDiary.Infrastructure.Persistence.Mapping

open GymDiary.Application.Workflows.Validation
open GymDiary.Infrastructure.Persistence.Documents

type IDocumentMapper<'TEntity, 'TDocument when 'TDocument :> IDocument> =
    abstract member MapFromDomain: 'TEntity -> 'TDocument
    abstract member MapToDomain: 'TDocument -> Result<'TEntity, ValidationError>
