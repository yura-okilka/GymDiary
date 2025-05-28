namespace GymDiary.Infrastructure.Persistence.Documents

open System

type IDocument =
    abstract member Id: string
    abstract member CreatedOnUtc: DateTime with get, set
    abstract member UpdatedOnUtc: DateTime with get, set

type IDocumentWithOwner =
    inherit IDocument
    abstract member OwnerId: string
