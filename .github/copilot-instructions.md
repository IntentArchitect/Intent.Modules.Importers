# Intent Modules Importers Guidance

Use these rules when working in this repository:

## 🎯 Element Lookup & Resolution Standard

When resolving or matching elements (types, classes, enums, type definitions, etc.), apply this **precedence order**:

1. **ExternalReference match** — First priority. Lookup by exact external reference (compiled fully-qualified identifier).  
   *Why: Stable across re-imports; preserves idempotency and existing element IDs.*

2. **Type name/specialization match** — Second priority. For type definitions specifically, lookup by name + generic parameter count.  
   *Why: Type definitions are identified by semantic name, not syntax.*

3. **Element name match** — Last priority (fallback). Lookup by display/canonical name within the current scope.  
   *Why: Weakest signal; use only when external reference and type specialization fail.*

4. **Create new if no match** — All lookups exhausted.  
   *Why: Ensures required elements exist; cascaded type definitions are created as needed.*

This order is enforced in `SetTypeReference()`, `GetElementByReference()`, `TryGetElementByName()`, and all importer-specific reference resolution logic. **Do not invert or skip tiers** in matching logic; doing so breaks re-import stability.

---

## 🔄 Core Principles

- Treat re-import idempotency as a core invariant. Changes to importer behavior should preserve existing element and association IDs whenever an existing model item can still be matched.
- Preserve `ExternalReference` semantics. Do not change matching keys, generated reference formats, or precedence rules casually; most importer stability depends on them.
- Prefer fixing importer behavior in the real mapping or synchronization layer instead of patching model output after the fact.
- Keep task entrypoints thin. Validate request models early, log the input and output shape, and push behavior into analyzers, factories, visitors, synchronizers, or mergers.
- Preserve manually modeled metadata where the current behavior and tests expect it to survive re-imports.
- When changing importer behavior, update the closest importer-specific tests and add regression coverage for merge, deletion, collision, or precedence behavior as applicable.
- Reuse `TestData` object mothers, scenario composers, and snapshot builders instead of constructing large test graphs inline.
- Run focused tests for the touched importer, and prefer running `/run-pre-commit-checks.ps1` before concluding broader work.
- Do not generalize shared infrastructure unless the behavior is genuinely common across multiple importers.
- Treat the existing tests as the primary specification for merge semantics, naming, and model preservation rules.