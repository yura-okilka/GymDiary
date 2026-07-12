# GymDiary

## Persistence (MongoDB) type naming

Persistence types in `GymDiary.Persistence.MongoDB` mirror domain types of the same name, and
mappers `open` both namespaces — so every persistence type needs a suffix to avoid clashing with its
domain counterpart. Use two suffixes, by role:

- **`Document`** — a root aggregate persisted as its own collection document (e.g. `UserDocument`,
  `WorkoutDocument`).
- **`Dto`** — everything embedded inside a document: records, value shapes, **and enums** (e.g.
  `GenderDto`, `WeightedRepetitionDto`, `RoutineSnapshotDto`).

Use `Dto` uniformly for the embedded tier; do not give enums a separate suffix.
