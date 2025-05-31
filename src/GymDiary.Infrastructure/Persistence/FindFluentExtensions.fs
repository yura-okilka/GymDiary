namespace GymDiary.Infrastructure.Persistence

open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Common.Extensions
open MongoDB.Driver

[<Extension>]
type FindFluentExtensions() =
    [<Extension>]
    static member SingleOrNoneAsync(find: IFindFluent<'TDocument, 'TProjection>, ?ct: CancellationToken) : Task<'TProjection option> = task {
        let! projection = find.SingleOrDefaultAsync(defaultArg ct CancellationToken.None)
        return Option.ofRecord projection // TODO: ofObj?
    }
