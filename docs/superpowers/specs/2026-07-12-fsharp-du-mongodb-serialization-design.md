# Design: Production-ready F# DU BSON support + `ExerciseSetGroup` persistence redesign

**Date:** 2026-07-12
**Status:** Approved (design)
**Scope:** `MongoDB.FSharp.Serialization`, `GymDiary.Persistence.MongoDB`

## Problem

F# discriminated unions have no first-class BSON serialization in this codebase.
`MongoDB.FSharp.Serialization` currently supports only `option` and `list`. Because of
that gap, the domain DU `ExerciseSetGroup` (5 cases, tuple payloads) is persisted by
hand-flattening it into a `ExerciseSetDto list`, where each set carries a `Kind` enum
tag (`ExerciseSetDtoMapper`).

That mapper reintroduces, at read time, invariants the DU makes impossible **by
construction**:

- *"All sets must be of the same kind"* — impossible in the domain (kind is lifted to the
  whole group), but the flat DTO can express a mixed list, so the mapper must re-check it.
- *"`<x>` is not a valid kind"* — the enum can hold values the domain cannot.

This is the smell to remove. The fix is genuine DU serialization so the persistence
representation mirrors the domain's algebra, and the mapper collapses to a trivial
structural map.

## Decisions (locked)

- **Build our own**, TypeShape-based, inside `MongoDB.FSharp.Serialization`. FSharp.MongoDB
  (fsprojects) and TypeShape (eiriktsarpalis) are design references only; no runtime
  dependency on FSharp.MongoDB.
- **DTO DU mirrors the domain type**, redesigned as needed. Persistence types are plain
  (`int`, `float`, `TimeSpan`); tuple payloads become named records so the stored schema is
  named and queryable.
- **Discriminator field:** `_t` (driver-idiomatic, queryable, aligns with
  `ScalarDiscriminatorConvention`).
- **New shape only.** No legacy data to preserve; the serializer reads/writes only the new
  tagged-DU shape. No migration, no back-compat reader.
- **Drop the empty-set-list check** from the mapper. Non-emptiness is an `ExerciseDefinition`
  invariant enforced at domain construction; the persistence mapper trusts stored data.

## Component 1 — Generic DU serializer (`MongoDB.FSharp.Serialization`)

A new `DiscriminatedUnionSerializer<'T>` built with TypeShape (`Shape.FSharpUnion`). Fully
generic; `ExerciseSetGroupDto` is merely its first consumer.

### BSON shape

- Scalar discriminator `_t` = union case name, written first.
- Each case field written under its **camelCased label**; value delegated to
  `BsonSerializer.LookupSerializer(fieldType)` so nested records / lists / options compose.
- Nullary case → `{ "_t": "CaseName" }`.

Example (`WeightedRepetitionSets [ (5, 20.0<kg>); (8, 22.5<kg>) ]`):

```json
{
  "_t": "WeightedRepetitionSets",
  "sets": [
    { "repetitions": 5, "weight": 20.0 },
    { "repetitions": 8, "weight": 22.5 }
  ]
}
```

### Behavior

- **Serialize:** TypeShape `GetTag` selects the case; iterate its `Fields` (typed getters);
  write `_t`, then each field by camelCased label via its looked-up serializer.
- **Deserialize:** buffer the document, read `_t`, match the case by name, read each field by
  label (order-independent), construct the value via TypeShape's union-case constructor.
- **Covered by construction:** nullary / single-field / multi-field cases; named *and*
  unnamed (`Item`, `Item1`, `Item2`) fields; single-case DUs; arbitrary nesting.

### Wiring

- `TypeUtils.fs`: add `isUnion` predicate (`FSharpType.IsUnion`).
- `FSharpTypeSerializers.fs` (`FSharpTypeSerializationProvider.GetSerializer`): add
  `elif isUnion objType then <DU serializer>` **after** the existing `option` and `list`
  branches — those are DUs too and must keep their specialized serializers.
- `camelCase` helper mirrors `CamelCaseElementNameConvention` so DU field names match the
  rest of the schema.
- Add `TypeShape` `PackageReference` and register the new `Serializers/DiscriminatedUnionSerializer.fs`
  in `MongoDB.FSharp.Serialization.fsproj`.
- `SerializationSettings.register` needs **no change** — it already calls
  `FSharpTypeSerializers.register`, which now includes DU support.

## Component 2 — Redesigned persistence DTO (`Documents/ExerciseSetsDto.fs`)

Remove `ExerciseSetKindDto` (enum) and the flat `ExerciseSetDto`. Replace with a DU that
mirrors `ExerciseSetGroup` case-for-case, using plain persistence types and **named union
fields** (so BSON field names are meaningful rather than `item`):

```fsharp
namespace GymDiary.Persistence.MongoDB.Documents

open System

[<CLIMutable>] type WeightedRepetitionDto = { Repetitions: int; Weight: float }
[<CLIMutable>] type WeightedDurationDto  = { Duration: TimeSpan; Weight: float }
[<CLIMutable>] type TimedDistanceDto     = { Distance: float; Duration: TimeSpan }

type ExerciseSetGroupDto =
    | RepetitionSets         of repetitions: int list
    | WeightedRepetitionSets of sets: WeightedRepetitionDto list
    | DurationSets           of durations: TimeSpan list
    | WeightedDurationSets   of sets: WeightedDurationDto list
    | TimedDistanceSets      of sets: TimedDistanceDto list
```

Notes:

- Units of measure (`float<kg>`, `float<m>`) and UMX tags (`Guid<'m>`) are erased at runtime,
  so plain `int` / `float` is the correct persistence representation.
- Payload records stay `[<CLIMutable>]` — the established DTO convention here — so the
  driver's class map serializes them; the DU serializer delegates to it.

## Component 3 — Redesigned mapper (`Mapping/ExerciseSetGroupMapper.fs`)

Replaces `ExerciseSetDtoMapper`. Still implements
`IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto>`, now a trivial structural
case-to-case map.

- **`MapFromDomain`**: one arm per case; unwrap `PositiveInt.Value`, `float w` for weights,
  `float dist` for distances into the matching DTO record/list.
- **`MapToDomain`**: one arm per case; reconstruct constrained primitives (`PositiveInt.create`
  via the existing `Validation.checkField` helper) and UMX-tag floats (`floatKg` / `floatM`).
  Returns `Result<ExerciseSetGroup, ValidationError>`.
- **Deleted checks:** "all sets same kind", "valid kind" (impossible by construction now) and
  "at least one set" (per locked decision).

The mapper remains a shared component because `WorkoutDocument` persistence will reuse it.

## Component 4 — Wire-up changes

- `Documents/ExerciseDefinitionDocument.fs`: `Sets: ExerciseSetDto list` → `Sets: ExerciseSetGroupDto`.
- `Documents/WorkoutDocument.fs`: `ExerciseDefinitionSnapshotDto.Sets` and `ExerciseDto.Sets`
  → `ExerciseSetGroupDto` (type-only; no workout mapper exists yet on this branch).
- `Mapping/ExerciseDefinitionDocumentMapper.fs`: constructor dependency becomes
  `IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto>`; `MapFromDomain` / `MapToDomain`
  call sites unchanged (`setsMapper.MapFromDomain` now returns a single DU value matching the
  new field type).
- `PersistenceBuilderExtensions.fs`: register
  `IDocumentMapper<ExerciseSetGroup, ExerciseSetGroupDto>, ExerciseSetGroupMapper`.
- Delete `Mapping/ExerciseSetDtoMapper.fs`.
- Keep `EnumRepresentationConvention(BsonType.String)` in `SerializationSettings` (harmless;
  available for future enums).

## Component 5 — Testing (TDD order)

1. **Serializer round-trip tests first.** Sample DUs covering every edge case — nullary,
   single-field, multi-field, unnamed `Item` field, single-case DU, nested record / list /
   option — asserting both the exact BSON shape and value round-trip. Home: a small dedicated
   `MongoDB.FSharp.Serialization.UnitTests` project (the library is standalone and reusable).
2. Rewrite `ExerciseSetDtoMapperTests` → `ExerciseSetGroupMapperTests`: per-case structural
   round-trip plus a validation-failure case (invalid reps → `Error`).
3. Update `ExerciseDefinitionDocumentMapperTests` for the new `Sets` type.
4. Optional integration test (`GymDiary.IntegrationTests`): persist → fetch an
   `ExerciseDefinition` against real MongoDB and assert the stored `sets` sub-document shape.

## Non-goals

- No migration / legacy-shape reader (new shape only).
- No moving the existing `option` / `list` serializers onto TypeShape (leave as-is; YAGNI).
- No generic F# record serializer in the library — records continue to serialize via the
  driver's class map (`[<CLIMutable>]`).
- No domain changes to `ExerciseSetGroup` — it keeps its tuple payloads; only the DTO uses
  records.

## Risk / verification notes

- Provider ordering is load-bearing: `option` and `list` are unions; the `isUnion` branch
  must come last so it never shadows their serializers.
- The DU serializer writes field names itself, so camelCasing must be applied in the
  serializer (the member-map convention does not reach it).
- Deserialization must be field-order-independent (buffer the document; do not assume `_t`
  is physically first).
```
