namespace GymDiary.Infrastructure.Persistence.Documents

type IDocument =
    abstract member Id: string

type IDocumentWithOwner =
    inherit IDocument
    abstract member OwnerId: string
