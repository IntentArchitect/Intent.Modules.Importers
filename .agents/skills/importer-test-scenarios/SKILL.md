---
name: importer-test-scenarios
description: 'Use when adding or restructuring tests for CSharp, Json, OpenApi, or Rdbms importers. Helps create scenario-driven tests with TestData object mothers, snapshots where appropriate, and assertions aligned to importer semantics.'
---

# Importer Test Scenarios Skill

Use this skill when you need to add new importer tests or reorganize test coverage.

## Testing Style In This Repo

- Behavioral tests use direct assertions for a few critical guarantees.
- Snapshot tests verify broad mapped output shapes.
- `TestData` folders hold object mothers and reusable scenarios.
- Re-import and merge tests are first-class, not edge cases.

## Workflow

1. Classify the scenario.

- Mapping-only: source input to persistables or model elements.
- Merge/re-import: existing package plus new import.
- Collision/deduplication: duplicate names, paths, schemas, or references.
- Integration: CLI or container-backed extraction.

2. Choose the right test shape.

- Use targeted assertions for one or two behavioral guarantees.
- Use snapshot tests when many properties, stereotypes, or nested structures matter.

3. Build fixtures the repo way.

- Extend `TestData` helpers before creating inline graphs.
- Reuse package factories, config factories, schema builders, code samples, and scenario composers.

4. Assert the important importer semantics.

- Names, specializations, stereotypes, associations, folder placement, and type references.
- Identity preservation for re-import scenarios.
- Absence of duplicates for collision scenarios.
- Correct behavior under deletion-enabled and deletion-disabled modes where relevant.

5. Keep the test readable.

- One scenario per test.
- Name the test around the input condition and expected outcome.
- Avoid large inline setup blocks when a `TestData` helper would make intent clearer.

## Good Triggers For This Skill

- New importer behavior needs coverage.
- Existing test setup is too ad hoc or duplicated.
- A bug involves duplicate fields, repeated imports, schema precedence, or snapshot drift.
- A change affects how existing package models are updated rather than created from scratch.