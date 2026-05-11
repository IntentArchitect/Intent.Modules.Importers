---
applyTo: 'Modules/**/*.Tests/**/*.cs,Tests/**/*.cs'
description: 'Use when adding or editing importer tests. Prefer object mothers in TestData, explicit scenario-based names, idempotency and merge regression coverage, and snapshots for broad structure verification.'
---

When writing tests in this repository:

- Prefer Arrange/Act/Assert structure with scenario-driven test names.
- Use `TestData` object mothers, package builders, schema builders, code samples, and scenario composers before creating ad hoc fixtures.
- Add explicit regression coverage for re-import stability, collision handling, deletion behavior, and manual-model preservation whenever matching logic changes.
- Use snapshot tests when the full mapped structure matters and targeted assertions when only a few guarantees matter.
- Keep tests close to the importer’s real semantics: `ExternalReference` matching, precedence rules, stereotypes, nullability, associations, and folder/schema placement.
- For merge behavior, verify both counts and identity preservation where applicable.
- For importer bugs involving duplicates, assert both absence of exceptions and absence of duplicate model members.
- Keep test data readable and purpose-specific; prefer adding new object-mother helpers over bloating a single giant fixture.