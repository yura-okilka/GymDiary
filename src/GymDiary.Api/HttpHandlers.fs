namespace GymDiary.Api

open Giraffe

[<RequireQualifiedAccess>]
module HttpHandlers =

    let pingPong: HttpHandler = noResponseCaching >=> text "pong"

    module ExerciseCategory =

        let create: HttpHandler = text "create"

        let getAll: HttpHandler = text "getAll"

        let getById (id: string) : HttpHandler = text $"getById %s{id}"

        let rename (id: string) : HttpHandler = text $"rename %s{id}"

        let delete (id: string) : HttpHandler = text $"delete %s{id}"
