# GymDiary

A workout-tracking backend built with **.NET 10**, following **Clean / Hexagonal
architecture** (ports & adapters). The domain and application core are written in
**F#** (functional domain modelling); the host, adapters, and tooling are in **C#**.
Orchestration and local infrastructure are handled by **.NET Aspire**.

## Solution layout

```
GymDiary.Domain ──────────────┐                 (core, no dependencies)
                              ▼
GymDiary.Application ──────────┐  defines PORTS  (depends on Domain only)
        ▲                      ▼
        │              GymDiary.Persistence.MongoDB   ADAPTER (implements ports)
        │
GymDiary.Api  ── composition root + HTTP driving adapter (depends on Application
                 + adapters; maps results to HTTP)

Supporting:
  Common                       small shared F# helpers (no domain knowledge)
  GymDiary.ApiClient           Kiota-generated typed client (from the API's OpenAPI)
  GymDiary.AppHost             Aspire orchestration (Mongo container + API)
  GymDiary.ServiceDefaults     Aspire cross-cutting (telemetry, health, resilience)
  MongoDB.FSharp.Serialization F# <-> BSON serialization support
```

**The dependency rule:** dependencies point *inward*. `Domain` depends on nothing;
`Application` depends only on `Domain`; adapters (`Persistence.MongoDB`, `Api`) depend
on `Application` and implement the abstractions it declares. Nothing the core depends
on knows about HTTP, MongoDB, or any other technology.

## Layers and their rules

### `GymDiary.Domain` (F#)
The heart of the system: entities, value objects, and the rules that must always hold.

- **No project dependencies** (only `FSharp.Core`). Knows nothing about persistence,
  HTTP, DI, or any framework.
- **Make illegal states unrepresentable.** Value objects use smart constructors with
  `private` union cases (`String50`, `EmailAddress`, …) and validate on creation,
  returning `Result<_, _>`.
- Strongly-typed identifiers via the phantom type `Id<'T>` (e.g. `Id<User>`).
- Units of measure for physical quantities (`float<kg>`, `float<m>`).
- Aggregates are records with a companion `module` exposing `create` / `update`
  functions that enforce invariants.

### `GymDiary.Application` (F#)
Use cases and the **ports** (abstractions) the core needs from the outside world.

- **Depends only on `Domain`.**
- **Ports** (interfaces implemented by adapters), grouped by concern:
  - `Persistence/` — the repository interfaces
  - `Identity/` — `ICurrentUser` (who the operation runs as)
  - `Time/` — a `TimeProvider.UtcNow` extension (no custom clock interface)
  - `Workflows/` — `IRequestHandler<'TRequest, 'TResponse, 'TError>`
- **Workflows** = one module per use case, each with a `Command`/`Query`, a typed
  error DU (`CommandError`/`QueryError`), and a `Handler` implementing
  `IRequestHandler`, returning `Task<Result<_, _>>`. Input is validated with the
  `validation { }` computation expression; the flow is railway-oriented
  (`asyncResult { }`).
- Never references a concrete technology — it asks for ports, the host injects
  implementations.

### `GymDiary.Persistence.MongoDB` (F#) — adapter
The MongoDB implementation of the persistence ports.

- **Implements** `GymDiary.Application` abstractions (the repository interfaces).
- Owns MongoDB documents, document↔domain mappers, the Mongo context, and
  serialization settings. The rest of the system never sees a `BsonDocument`.
- Exposes a single DI entry point: `AddGymDiaryMongoDB()`.

### `GymDiary.Api` (C#) — composition root & driving adapter
The ASP.NET Core host that turns HTTP requests into workflow calls.

- **Composition root:** the only place that wires concrete adapters to ports
  (`Program.cs`).
- **Minimal API** endpoints implement `IEndpoint`; they're discovered by assembly
  scan and mapped via `MapEndpoints()`.
- Translates workflow `Result`s to HTTP: success → `TypedResults.Ok/Created`,
  typed errors → `ProblemDetails` / `ValidationProblem` at the edge.
- Implements `ICurrentUser` (`HttpCurrentUser`; `FakeCurrentUser` until real auth is
  wired up).
- Emits an OpenAPI document at build time, which drives `GymDiary.ApiClient`.

## Conventions

- **Language split:** core (`Domain`, `Application`) and adapters that benefit from it
  (`Persistence.MongoDB`) are F#; host/tooling (`Api`, `AppHost`, `ServiceDefaults`,
  `ApiClient`) are C#.
- **Adapter project naming:** `GymDiary.<Concern>.<Technology>` — e.g.
  `GymDiary.Persistence.MongoDB`. Future adapters slot in as
  `GymDiary.Identity.Keycloak`, `GymDiary.Messaging.RabbitMQ`, each pairing with a
  `GymDiary.Application.<Concern>` port.
- **DI registration:** every module exposes an `AddGymDiary<X>` extension, composed in
  `Program.cs`. Adapters are named by **technology** (`AddGymDiaryMongoDB`); core
  layers by name (`AddGymDiaryApplication`).
- **Errors are typed, not strings/exceptions.** Domain and workflow errors are
  discriminated unions, threaded through `Result`/`asyncResult`, and mapped to HTTP
  only at the API boundary.
- **HTTP contracts** are records in `Api/Contracts/`, named by role
  (`ExerciseCategoryResponse`, `Request`/`Response`), kept separate from domain types.
- **No reflection-based mapping** between layers — explicit mappers/constructors.

## Testing

- **`GymDiary.Domain.UnitTests`** — xUnit + FsUnit; fast, pure unit tests of domain
  rules and value objects.
- **`GymDiary.IntegrationTests`** — boots the real app via the Aspire `AppHost`
  (with a MongoDB container) and exercises it through the generated `GymDiary.ApiClient`.

## Building & running

```bash
dotnet build                                   # whole solution (net10.0)
dotnet run --project src/GymDiary.AppHost      # run via Aspire (starts Mongo + API)
dotnet test                                    # all tests
```

Repo-wide settings live in `Directory.Build.props` (`net10.0`, nullable enabled,
warnings-as-errors) and `Directory.Packages.props` (central package management).

## `archive/`

The `/archive/` solution folder holds the previous stack — an Oxpecker-based API
(`GymDiary.Api.Oxpecker`), its core (`GymDiary.Core`), DB migrations, and the older
system tests — kept for reference while the new architecture is built out. New work
targets the `/src/` projects above.
