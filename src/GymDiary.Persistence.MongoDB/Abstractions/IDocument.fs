namespace GymDiary.Persistence.MongoDB.Documents

open System

type IDocument =
    abstract member Id: Guid

type IDocumentWithOwner =
    inherit IDocument
    abstract member OwnerId: Guid
