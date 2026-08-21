# Fix: "Preserve user-specified attribute types" ignored for stored-procedure return types

## Context

`GetValueSelect` never executes a `SELECT` or an output parameter, so on the very first import its
return type is correctly detected as void. The user then models the return type as `int?` by hand.
On re-import, even with **"Preserve user-specified attribute types" checked** (the default), that
`int?` is wiped back to void.

Root cause, confirmed by reading `DbSchemaIntentMetadataMerger.cs` directly:

- Stored-procedure return-type detection (`Intent.RelationalDbSchemaImporter.CLI`) has exactly two
  outcomes: the analyzer call (`sp_describe_first_result_set`) either **throws** (→
  `StoredProcedureSchema.ResultSetDetectionFailed = true`, "indeterminate this run") or **succeeds**
  with some column count (0 columns + no throw → "conclusively void"). There is no static analysis of
  the procedure body, so a proc that structurally never returns anything looks identical, to this
  code, to a proc whose `SELECT` was *removed* since the last import.
- Three call sites — `ProcessStoredProcedureStandard` (line 327), and
  `ProcessStoredProcedureWithOperationMapping` (lines 461 and 601) — each have:
  ```csharp
  else if (!storedProc.ResultSetDetectionFailed)
  {
      procElement.TypeReference.TypeId = null;
      procElement.TypeReference.IsCollection = false;
  }
  ```
  This runs **after** `SyncElements` has already applied (or not applied) attribute-type
  preservation, and it is **not gated by `_config.PreserveAttributeTypes` at all** — it always wins.
  This was deliberately added in commit `398a98f` to handle the case where a procedure used to have a
  detected result set and the DB now conclusively reports none (a real schema change) — three tests
  (`..._WithDefaultPreserveAttributeTypes`, lines 1414/1446/1478 in
  `DbSchemaIntentMetadataMergerTests.cs`) explicitly lock in "clears even with preservation enabled."
- Separately, the *general* attribute-type-preservation warning was already scaffolded but left
  commented out in `InternSyncElements` (same file, ~line 756‑766), with the comment "I want a nice
  notification here" — it also references a stale variable name that no longer exists, so it was
  never wired up for the generic (non-SP) attribute path either.

**The design fork this fix has to resolve:** because "conclusively void" and "structurally always
void" are indistinguishable with today's detection, honoring requirement 1 (keep `int?`) for
`GetValueSelect` necessarily also means a procedure that *used to* return rows and now doesn't will
**also** keep its stale return type whenever "Preserve user-specified attribute types" is checked —
reversing the specific guarantee added in `398a98f` for the default-on case. Unchecking the box still
gives fully DB-driven behavior (type clears immediately), which continues to satisfy that original
scenario. Given the checkbox's own documented contract ("will not overwrite **any** attribute type
set by the user") already promises exactly this, this fix treats that as the correct, intended
behavior rather than a regression — but it's flagged here explicitly since `398a98f` was authored by
the same user this fix now overrides by default. Worth a final nod of agreement before implementing,
since it does change re-import behavior for anyone relying on the current default-on auto-clear.

## Changes

### 1. `Modules\Intent.Modules.Rdbms.Importer\Tasks\Mappers\DbSchemaIntentMetadataMerger.cs`

At all three "genuinely void" clearing sites (line ~327 in `ProcessStoredProcedureStandard`; lines
~461 and ~601 in `ProcessStoredProcedureWithOperationMapping`), gate the clear on
`_config.PreserveAttributeTypes` **and** the already-computed `hadExistingReturnType` /
`spHadReturnType` / `operationHadReturnType` flag (true only when the existing element already had a
real return type, so a void→void re-import doesn't spam a warning). When preservation wins, emit a
warning via the existing `result.Warnings` list, mirroring the style of the sibling
`ResultSetDetectionFailed` warning already at lines 297/402/517:

```csharp
else if (!storedProc.ResultSetDetectionFailed)
{
    if (_config.PreserveAttributeTypes && hadExistingReturnType)
    {
        result.Warnings.Add($"Database reports no result set for '{storedProc.Schema}.{storedProc.Name}' this import; the existing modelled return type was preserved because 'Preserve user-specified attribute types' is enabled.");
    }
    else
    {
        procElement.TypeReference.TypeId = null;
        procElement.TypeReference.IsCollection = false;
    }
}
```

Apply the analogous change at the other two sites, using their respective `spHadReturnType` /
`operationHadReturnType` locals and element variables (`storedProcElement`, `operationElement`).

### 2. Generic attribute-type warning — same file, `InternSyncElements` (~line 745‑784)

Fix and enable the existing scaffolded-but-commented block so **every** attribute-level type
preservation (table columns, DTO/view fields, data-contract properties — anything routed through
`SyncElements`) logs a warning when it holds back a DB-detected type change, not just stored
procedures:

```csharp
if ((preserveElementTypes & SyncElementType.AttributeType) == 0 || string.IsNullOrEmpty(existingElement.TypeReference.TypeId))
{
    existingElement.TypeReference.TypeId = sourceElement.TypeReference.TypeId;
}
else if (existingElement.TypeReference.TypeId != sourceElement.TypeReference.TypeId)
{
    var elementIdentifier = string.IsNullOrEmpty(parentSchema)
        ? existingElement.Name
        : $"{parentSchema}.{existingElement.Name}";
    result?.Warnings.Add($"Preserved user-specified type for '{elementIdentifier}' during re-import (attribute type preservation is enabled).");
}
```

Note: the original commented-out draft referenced `sourceElement.TypeReference.TypeName`, which isn't
confirmed to exist on `TypeReferencePersistable` in this codebase (no other usage found) — dropped
from the message rather than guessed at; confirm during implementation whether a friendly type name
is available and worth adding back.

Both fixes reuse the existing `MergeResult.Warnings` (`List<string>`) plumbing — already surfaced to
the user via `Logging.Log.Warning(...)` in `DatabaseImport.Execute`, exactly like the other warnings
in this file (duplicate FK, removed attribute/index/association, detection-failed). No new plumbing
needed.

## Test changes (`Modules\Intent.Modules.Rdbms.Importer.Tests\DbSchemaIntentMetadataMergerTests.cs`)

- **New test** reproducing the exact reported bug: import a stored proc with `ResultSetColumns = []`
  and `ResultSetDetectionFailed = false` (void from the start, like `GetValueSelect`), manually set
  `existingElement.TypeReference.TypeId`/`IsNullable` on the resulting element (simulating the
  designer edit to `int?`), re-import the same void schema with default config
  (`PreserveAttributeTypes = true`), and assert the manually-set type survives **and**
  `result.Warnings` contains the new "existing modelled return type was preserved" message. Add one
  variant per strategy (`StoredProceduresAsOperations`, `StoredProceduresAsElements`,
  `StoredProceduresMappedToOperation`) matching the existing three-strategy pattern used elsewhere in
  this file.
- **Update** the three existing `..._WithDefaultPreserveAttributeTypes` tests (lines 1414, 1446, 1478)
  — these currently assert clearing happens even with default preservation on; per the design-fork
  above they now must assert the opposite (type preserved + warning present). Rename to
  `..._PreservesReturnType_..._WithDefaultPreserveAttributeTypes` and update bodies/assertions
  accordingly.
- **Add** explicit `PreserveAttributeTypes = false` counterparts for the `StoredProcedureElement` and
  `RepositoryOperationMapping` strategies (the `RepositoryOperation` one already exists at line 1381)
  to lock in that unchecking the box still gives full DB-driven clearing.
- **Extend** `MergeSchemaAndPackage_AttributeWithCustomType_PreservesCustomTypeWhenConfigured` (line
  279) with a `result.Warnings.ShouldContain(...)` assertion for the new generic warning, following
  this repo's convention of only asserting warning text when the warning itself is the behavior under
  test (see `AGENTS.md` testing rules).
- **Verify** `MergeSchemaAndPackage_StoredProcGenuinelyVoidOnReimport_RemovesStaleResultMapping` (line
  1516) still passes unmodified — its assertions are on `association.TargetEnd.Mappings`, driven by
  `underlyingResultDataContract` being null (a separate, untouched code path), not on the
  element `TypeReference.TypeId` this fix changes. Confirm with a full test run rather than assuming.

## Verification

1. `dotnet build "Modules/Intent.Modules.Rdbms.Importer/Intent.Modules.Rdbms.Importer.csproj"` and the
   `.Tests` project (the `build` task already wired up).
2. `dotnet test "Modules/Intent.Modules.Rdbms.Importer.Tests/Intent.Modules.Rdbms.Importer.Tests.csproj"`
   — full suite, not just the new/changed tests, since this touches a shared code path
   (`InternSyncElements`) used by every importer strategy.
3. Run `./run-pre-commit-checks.ps1` per this repo's standing guidance before considering the work done.
